using RightFoldPattens.Logging;
using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using System;
using System.Collections.Generic;

namespace RightFoldPatterns.Stratesy
{
    /// <summary>
    /// 複数の ILineProcessor をパイプラインとして合成
    /// </summary>
    public class CompositeStrategy : ILineProcessor
    {
        private readonly IReadOnlyList<ILineProcessor> _pipeline;

        public CompositeStrategy(params ILineProcessor[] processors)
        {
            if (processors == null || processors.Length == 0)
                throw new ArgumentException("パイプラインは1つ以上必要です。", nameof(processors));

            _pipeline = processors;
        }

        /// <summary>
        /// パイプラインを順に処理
        /// </summary>
        /// <param name="line">入力ログ</param>
        /// <returns>変換後のログ</returns>
        public ILogLine Process(ILogLine line)
        {
            ILogLine current = line;

            foreach (var processor in _pipeline)
            {
                if (processor == null)
                    continue;

                current = processor.Process(current) ?? current;
            }

            return current;
        }
    }
}
