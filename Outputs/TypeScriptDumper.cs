using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var stringBuilder = new StringBuilder();

            foreach (var line in DumpProto(proto)) {
                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }

        public override List<string> DumpProto(Proto proto) {
            var strings = new List<string>();

            var imports = new List<string>();

            // TODO: Make a decent way to get imports because this..................
            foreach (var field in proto.Fields) {
                foreach (var type in field.Types) {
                    if (type.IsImport) imports.Add(type.Name);
                }
            }

            foreach (var nestedProto in proto.NestedProtos) {
                foreach (var field in nestedProto.Fields) {
                    foreach (var type in field.Types) {
                        if (type.IsImport) imports.Add(type.Name);
                    }
                }
            }

            foreach (var oneof in proto.Oneofs) {
                foreach (var entry in oneof.Entries) {
                    if (entry.IsImport) imports.Add(entry.Type);
                }
            }

            if (!proto.Nested) {
                foreach (var import in imports.Distinct().ToList()) {
                    strings.Add($"import {{ {import} }} from \"./{import}\";");
                }
                if (imports.Count > 0) strings.Add("");
            }

            foreach (var pEnum in proto.Enums) {
                foreach (var line in DumpEnum(pEnum)) {
                    strings.Add(line);
                }
                strings.Add("");
            }

            foreach (var nestedProto in proto.NestedProtos) {
                foreach (var line in DumpProto(nestedProto)) {
                    strings.Add(line);
                }
                strings.Add("");
            }

            strings.Add($"export {(proto.IsEnum ? "enum" : "interface")} {proto.Name} {{");

            foreach (var oneof in proto.Oneofs) {
                foreach (var entry in oneof.Entries) {
                    var isTypeNestedProtoOrEnum = false;
                    foreach (var nestedProto in proto.NestedProtos) {
                        if (nestedProto.Name == entry.Type) {
                            isTypeNestedProtoOrEnum = true;
                            break;
                        }
                    }
                    foreach (var pEnum in proto.Enums) {
                        if (pEnum.Name == entry.Type) {
                            isTypeNestedProtoOrEnum = true;
                            break;
                        }
                    }
                    strings.Add($"{ToCamelCase(entry.Name)}?: {ProtoTypeToLanguageType(entry.Type, isTypeNestedProtoOrEnum ? true : entry.IsImport)};");
                }
            }

            for (int i = 0; i < proto.Fields.Count; i++) {
                var field = proto.Fields[i];
                if (proto.IsEnum) {
                    strings.Add($"  {field.Types[0].Name} = {field.FieldNumber}{((i + 1) != proto.Fields.Count ? "," : "")}");
                }
                else {
                    if (field.IsRepeated) {
                        var isTypeNestedProtoOrEnum = false;
                        foreach (var nestedProto in proto.NestedProtos) {
                            if (nestedProto.Name == field.Types[0].Name) {
                                isTypeNestedProtoOrEnum = true;
                                break;
                            }
                        }
                        foreach (var pEnum in proto.Enums) {
                            if (pEnum.Name == field.Types[0].Name) {
                                isTypeNestedProtoOrEnum = true;
                                break;
                            }
                        }
                        strings.Add($"  {(field.Name != "" ? $"{ToCamelCase(field.Name)}" : "")}?: {ProtoTypeToLanguageType(field.Types[0].Name, isTypeNestedProtoOrEnum ? true : field.Types[0].IsImport)}[];");
                    }
                    else if (field.IsMap) {
                        var isType1NestedProtoOrEnum = false;
                        var isType2NestedProtoOrEnum = false;
                        foreach (var nestedProto in proto.NestedProtos) {
                            if (nestedProto.Name == field.Types[0].Name) {
                                isType1NestedProtoOrEnum = true;
                            }
                            if (nestedProto.Name == field.Types[1].Name) {
                                isType2NestedProtoOrEnum = true;
                            }
                        }
                        foreach (var pEnum in proto.Enums) {
                            if (pEnum.Name == field.Types[0].Name) {
                                isType1NestedProtoOrEnum = true;
                            }
                            if (pEnum.Name == field.Types[1].Name) {
                                isType2NestedProtoOrEnum = true;
                            }
                        }
                        strings.Add($"  {(field.Name != "" ? $"{ToCamelCase(field.Name)}" : "")}?: {{ [_: {ProtoTypeToLanguageType(field.Types[0].Name, isType1NestedProtoOrEnum ? true : field.Types[0].IsImport)}]: {ProtoTypeToLanguageType(field.Types[1].Name, isType2NestedProtoOrEnum ? true : field.Types[1].IsImport)} }};");
                    }
                    else {
                        var isTypeNestedProtoOrEnum = false;
                        foreach (var nestedProto in proto.NestedProtos) {
                            if (nestedProto.Name == field.Types[0].Name) {
                                isTypeNestedProtoOrEnum = true;
                                break;
                            }
                        }
                        foreach (var pEnum in proto.Enums) {
                            if (pEnum.Name == field.Types[0].Name) {
                                isTypeNestedProtoOrEnum = true;
                                break;
                            }
                        }
                        strings.Add($"  {(field.Name != "" ? $"{ToCamelCase(field.Name)}" : "")}?: {ProtoTypeToLanguageType(field.Types[0].Name, isTypeNestedProtoOrEnum ? true : field.Types[0].IsImport)};");
                    }
                    
                }
            }

            strings.Add("}");

            return strings;
        }

        public override List<string> DumpEnum(ProtoEnum pEnum) {
            var strings = new List<string>();

            strings.Add($"export enum {pEnum.Name} {{");

            for (int i = 0; i < pEnum.Entries.Count; i++) {
                var entry = pEnum.Entries[i];
                strings.Add($"  {entry.Name} = {entry.Value}{((i + 1) != pEnum.Entries.Count ? "," : "")}");
            }
            strings.Add("}");

            return strings;
        }

        public static Dictionary<string, string> TypeScriptTypes = new Dictionary<string, string> {
            ["uint32"] = "number",
            ["uint64"] = "number",
            ["bool"] = "boolean",
            ["int32"] = "number",
            ["int64"] = "BigInt",
            ["string"] = "string",
            ["float"] = "number",
            ["double"] = "number",
            ["bytes"] = "Buffer",
            ["fixed32"] = "number"
        };

        public string ProtoTypeToLanguageType(string type, bool isImport) {
            if (isImport) return type;
            if (TypeScriptTypes.TryGetValue(type, out var typescriptType)) {
                return typescriptType;
            }
            else {
                Console.WriteLine($"Unknown type \"{type}\" found!");
                return $"UNK_{type}";
            }
        }
    }
}
