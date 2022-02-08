using System.Collections.Generic;

namespace ProtoDumper.Outputs {

    // WIP because I'm lazy
    public class TypeScriptDumper : BaseDumper {
        private List<Proto> Protos;
        private string OutputFolder;

        public TypeScriptDumper(List<Proto> protos, string outputFolder) : base(protos, outputFolder) {
            Protos = protos;
            OutputFolder = outputFolder;
        }

        public override string BuildFile(Proto proto) {
            return "";
        }

        public override List<string> DumpProto(Proto proto) {
            var strings = new List<string>();
            return strings;
        }

        public override List<string> DumpEnum(ProtoEnum pEnum) {
            var strings = new List<string>();
            return strings;
        }

        public override List<string> DumpOneof(ProtoOneof oneof) {
            var strings = new List<string>();
            return strings;
        }
    }
}
