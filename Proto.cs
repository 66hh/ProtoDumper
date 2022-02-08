using System.Collections.Generic;

namespace ProtoDumper {
    public class Proto {
        public string Name;
        public int CmdID;
        public List<ProtoField> Fields;
        public List<ProtoEnum> Enums;
        public List<Proto> NestedProtos;
        public List<ProtoOneof> Oneofs;
        public List<string> Imports;
        public bool Nested;
        public bool IsEnum;

        public Proto(string name, int cmdId, List<ProtoField> fields, List<ProtoEnum> enums, List<Proto> nestedProtos, List<ProtoOneof> oneofs, List<string> imports, bool nested, bool isEnum) {
            Name = name;
            CmdID = cmdId;
            Fields = fields;
            Enums = enums;
            NestedProtos = nestedProtos;
            Oneofs = oneofs;
            Imports = imports;
            Nested = nested;
            IsEnum = isEnum;
        }
    }

    public class ProtoField {
        public string Type;
        public string Name;
        public int FieldNumber;

        public ProtoField(string type, string name, int fieldNumber) {
            Type = type;
            Name = name;
            FieldNumber = fieldNumber;
        }
    }

    public class ProtoEnum {
        public string Name;
        public List<ProtoEnumEntry> Entries;

        public ProtoEnum(string name, List<ProtoEnumEntry> entries) {
            Name = name;
            Entries = entries;
        }
    }

    public class ProtoEnumEntry {
        public string Name;
        public int Value;

        public ProtoEnumEntry(string name, int value) {
            Name = name;
            Value = value;
        }
    }

    public class ProtoOneof {
        public string Name;
        public List<ProtoOneofEntry> Entries;

        public ProtoOneof(string name, List<ProtoOneofEntry> entries) {
            Name = name;
            Entries = entries;
        }
    }

    public class ProtoOneofEntry {
        public string Type;
        public string Name;
        public int Value;

        public ProtoOneofEntry(string type, string name, int value) {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}
