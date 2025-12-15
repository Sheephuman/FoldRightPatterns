using RightFoldPattens.Logging;
using RightFoldPatterns.Stratesy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RightFoldPatterns.Implements
{
    /// <summary>
    /// WPF用 ViewModel向けの文字列版 LazyLineEvaluator
    /// FoldRight（右畳み込み）構造を忠実に再現
    /// </summary>
    public class LazyLineEvaluator_FoldRightNonStrict
    {
        private readonly IEnumerable<string> _lines;

        // EvaluateNext 用の列挙子
        private readonly IEnumerator<string> _enumerator;

        private readonly List<LineStrategy> _strategies = new();

        public LazyLineEvaluator_FoldRightNonStrict(IEnumerable<string> lines)
        {
            _lines = lines ?? throw new ArgumentNullException(nameof(lines));
            _enumerator = _lines.GetEnumerator();
        }

        /// <summary>
        /// 条件＋Factoryを登録
        /// </summary>
        public void AddStrategy(Func<string, bool> condition, Func<string, ILogLine> factory)
        {
            _strategies.Add(new LineStrategy(condition, factory));
        }

        /// <summary>
        /// 最初に条件に一致した行だけ処理して終了
        /// 右畳み込み風遅延評価
        /// </summary>
          // EvaluateFirst を右畳み込み＋遅延評価で作り直し
    public bool EvaluateFirst(Func<string, bool> condition, Action<string> onMatch)
    {
        if (_enumerator is null) return false;

        while (_enumerator.MoveNext())
        {
            var line = _enumerator.Current;
            if (condition(line))
            {
                onMatch(line);
                return true;
            }
        }

        return false; // 条件に合う行なし
    }

        /// <summary>
        /// 次のマッチ行を返す（EvaluateNext風）
        /// </summary>
        public ILogLine? EvaluateNext()
        {
            while (_enumerator.MoveNext())
            {
                var line = _enumerator.Current;

                foreach (var s in _strategies)
                {
                    bool ok;
                    try { ok = s.Condition(line); }
                    catch { ok = false; }

                    if (ok)
                    {
                        try { return s.Factory(line); }
                        catch { return new SimpleLogLine(line, LogSeverity.Normal); }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 再構築版：スタックを一切使わない右畳み込み（完全非再帰）
        /// </summary>

        public IEnumerable<TResult> EvaluateAllRight<TResult>(
      Func<string, bool> condition,
      Func<string, TResult> factory)
        {
            var list = _lines.ToList();
            int index = list.Count - 1;

            // 継続たちを蓄積していく（StackOverflow のスタックとは別物）
            var continuations = new Stack<Func<IEnumerable<TResult>>>();

            // 右側の構造を先に貯める（本来 foldr がやること）
            while (index >= 0)
            {
                string line = list[index];
                if (condition(line))
                {
                    var value = factory(line);
                    continuations.Push(() => Prepend(value));
                }
                else
                {
                    continuations.Push(() => Enumerable.Empty<TResult>());
                }
                index--;
            }

            // ここから副次的に Cons 連鎖を作っていく
            IEnumerable<TResult> result = Enumerable.Empty<TResult>();
            while (continuations.Count > 0)
            {
                var build = continuations.Pop();
                result = build().Concat(result);
            }

            return result;
        }

        private static IEnumerable<T> Prepend<T>(T head)
        {
            yield return head;
        }

        /// <summary>
        /// foldr 構造を維持するための "Cons" 的構成子
        /// </summary>
        private static IEnumerable<T> Prepend<T>(T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach (var x in tail)
                yield return x;
        }




        /// <summary>
        /// EvaluateAllRightをAction形式で遅延評価する版
        /// </summary>
        public void EvaluateAll(Action<string> onMatch, Func<string, bool>? condition = null)
        {
            condition ??= (_ => true);

            var lines = _lines.ToList();
            void FoldRightRecursive(int index)
            {
                if (index < 0) return;

                var line = lines[index];
                if (condition(line))
                    onMatch(line);

                FoldRightRecursive(index - 1);
            }

            FoldRightRecursive(lines.Count - 1);
        }
    }
}
