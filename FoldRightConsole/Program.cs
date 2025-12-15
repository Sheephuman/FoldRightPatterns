using System;

class Program
{


    static void Main()
    {
     
            //遅延レイヤーなし版
            var it = FoldRigtFindResult.FoldRightSimple(0, 10000, 9000);
            while (it.MoveNext())
            {
                var value = it.Current;
                if (value != -1)
                {
                    Console.WriteLine("FoldRightSimple: " + value);
                }
            }

            //遅延レイヤーあり版（忠実な右畳み込み）
            var it2 = FoldRigtFindResult.FoldRightFind(0, 10000, 9000);
            while (it2.MoveNext()) 
            {
                var value2 = it2.Current;
                if (value2 != -1)
                {
                    Console.WriteLine("FoldRightFind: " + value2);
                }
            }
    }




    class FoldRigtFindResult
    {

        // 遅延レイヤーなし版：単に右側から走査する
        public static IEnumerator<int> FoldRightSimple(int start, int end, int threshold)
        {
            for (int i = end; i >= start; i--)
            {
                if (i <= threshold && i % 100 == 0)
                {
                    yield return i;   // 条件に合った値だけ返す
                    yield break;      // 最初に見つけた時点で終了
                }
            }

            yield return -1; // 見つからなかった場合の値
        }



        // 遅延レイヤーあり版：忠実な右畳み込み
        public static IEnumerator<int> FoldRightFind(int start, int end, int threshold)
        {
            return FoldRight(
                start,
                end,
                -1,                                  // seed
                (i, k) =>                             // f(i, rest)
                {
                    if (i >= threshold && i % 100 == 0)
                    {
                        return YieldOne(i, k);         // 見つけたら返して、あとは続く
                    }

                    return k;                          // 見つからなければ後続へ
                }
            );
        }


        /// <summary>
        /// 以下、右畳み込みの定義をC#で再現したもの
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="seed"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        static IEnumerator<int> FoldRight(int start, int end, int seed, Func<int, IEnumerator<int>, IEnumerator<int>> f)
        {
            IEnumerator<int> acc = YieldSeed(seed);

            for (int i = start; i <= end; i++)
            {
                acc = f(i, acc); // foldRight の形： f(i, acc)
            }

            return acc;
        }

        // 右畳み込みの “一番右端の層（シード）” を表す。
        // この IEnumerator は seed をひとつ返すだけのレイヤー。
        // foldRight の末尾に相当する。
        static IEnumerator<int> YieldSeed(int seed)
        {
            yield return seed;
        }


        // value を返したあと、後続レイヤー next を“続けて実行する”ための層。
        // foldRight でいう f(x, rest) の構造を C# で模倣している。
        // next はまだ評価されていない後続の処理で、MoveNext によって展開される。
        static IEnumerator<int> YieldOne(int value, IEnumerator<int> next)
        {
            // 現在のレイヤーとして value を吐く
            yield return value;

            // 後続レイヤーを、そのまま遅延的につなげて返す
            while (next.MoveNext())
                yield return next.Current;
        }

    }
}