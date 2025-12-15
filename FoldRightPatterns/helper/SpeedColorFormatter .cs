using RightFoldPattens.Logging;
using RightFoldPatterns.InterfaceDomain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;

namespace RightFoldPatterns.helper
{

        public class SpeedColorFormatter : ILineProcessor
        {

            private readonly IEnumerable<ILineProcessor> _pipeline;


            public SpeedColorFormatter(params ILineProcessor[] pipeline)
            {
                _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            }


        public ILogLine LogProcess(ILogLine line)
        {
            ILogLine current = line;
            foreach (var processor in _pipeline)
                current = processor.LogProcess(current);
            return current;
        }
    }

}
