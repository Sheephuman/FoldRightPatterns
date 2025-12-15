using RightFoldPattens.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.InterfaceDomain
{
    public interface ILineProcessor
    {
        /// <summary>
        /// 入力を受け取り、必要なら変更して返す
        /// </summary>
        ILogLine Process(ILogLine line);
    }
}
