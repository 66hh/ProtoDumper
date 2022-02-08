using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ProtoDumper.Outputs {
    public class BaseDumper {
        private List<Proto> Protos;
        private string OutputFolder;

        public BaseDumper(List<Proto> protos, string outputFolder) {
            Protos = protos;
            OutputFolder = outputFolder;
        }

        public virtual string BuildFile(Proto proto) {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("syntax = \"proto3\";");
            stringBuilder.AppendLine();
            foreach (var line in DumpProto(proto)) {
                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }

        public virtual List<string> DumpProto(Proto proto) {
            var strings = new List<string>();

            // TODO: Add a main instance file to add imports, this wouldn't work if there was a nested proto inside a nested proto, thankfully genshin doesn't have any
            foreach (var nestedProto in proto.NestedProtos) {
                foreach (var import in nestedProto.Imports) {
                    proto.Imports.Add(import);
                }
            }

            if (!proto.Nested) {
                foreach (var import in proto.Imports.Distinct().ToList()) {
                    strings.Add($"import \"{import}.proto\";");
                }
                if (proto.Imports.Count > 0) strings.Add("");
            }

            strings.Add($"{(proto.IsEnum ? "enum" : "message")} {proto.Name} {{");

            foreach (var nestedProto in proto.NestedProtos) {
                foreach (var line in DumpProto(nestedProto)) {
                    strings.Add("  " + line);
                }
            }

            foreach (var pEnum in proto.Enums) {
                foreach (var line in DumpEnum(pEnum)) {
                    strings.Add("  " + line);
                }
            }

            foreach (var oneof in proto.Oneofs) {
                foreach (var line in DumpOneof(oneof)) {
                    strings.Add("  " + line);
                }
            }

            foreach (var field in proto.Fields) {
                strings.Add($"  {field.Type}{(field.Name != "" ? $" {ToCamelCase(field.Name)}" : "")} = {field.FieldNumber};");
            }

            strings.Add("}");

            return strings;
        }

        public virtual List<string> DumpEnum(ProtoEnum pEnum) {
            var strings = new List<string>();

            strings.Add($"enum {pEnum.Name} {{");

            // TODO: Check if it has aliases, if it doesn't, don't add this line
            // TODO: Fix CmdId for protoc
            if (pEnum.Name.Equals("CmdId")) strings.Add("  option allow_alias = true;");
            foreach (var content in pEnum.Entries) {
                strings.Add($"  {content.Name} = {content.Value};");
            }
            strings.Add("}");

            return strings;
        }

        public virtual List<string> DumpOneof(ProtoOneof oneof) {
            var strings = new List<string>();

            strings.Add($"oneof {oneof.Name} {{");
            foreach (var entry in oneof.Entries) {
                strings.Add($"  {entry.Type} {ToCamelCase(entry.Name)} = {entry.Value};");
            }
            strings.Add("}");

            return strings;
        }

        public void Dump(string fileExtension) {
            var packetIds = new Dictionary<string, int>();

            foreach (var proto in Protos) {
                DumpToFolder(Path.Combine(OutputFolder, $"{proto.Name}.{fileExtension}"), BuildFile(proto));
                if (proto.CmdID != 0) packetIds.Add(proto.Name, proto.CmdID);
            }

            var orderedPackets = packetIds.OrderBy(p => p.Value).ToList();

            var writer = new StreamWriter(Path.Combine(OutputFolder, "packetIds.json"));
            var writerReversed = new StreamWriter(Path.Combine(OutputFolder, "packetIdsReversed.json"));

            writer.WriteLine("{");
            writerReversed.WriteLine("{");

            for (int i = 0; i < orderedPackets.Count; i++) {
                var packet = orderedPackets[i];
                writer.WriteLine($"  \"{packet.Key}\": \"{packet.Value}\"{((i + 1) != orderedPackets.Count ? "," : "")}");
                // Exclude DebugNotify because it has a duplicated ID
                if (packet.Key != "DebugNotify") writerReversed.WriteLine($"  \"{packet.Value}\": \"{packet.Key}\"{((i + 1) != orderedPackets.Count ? "," : "")}");
            }

            writer.WriteLine("}");
            writerReversed.WriteLine("}");

            writer.Close();
            writerReversed.Close();
        }

        public void DumpToFolder(string path, string content) {
            var writer = new StreamWriter(path);
            writer.Write(content);
            writer.Close();
        }

        public string ToCamelCase(string str) => string.IsNullOrEmpty(str) || str.Length < 2 ? str : char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
