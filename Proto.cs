using System.Collections.Generic;

namespace ProtoDumper {
    public class Proto {
        public string Name;
        public int CmdID;
        public List<ProtoField> Fields;
        public List<ProtoEnum> Enums;
        public List<Proto> NestedProtos;
        public List<ProtoOneof> Oneofs;
        public bool Nested;
        public bool IsEnum;

        public Proto(string name, int cmdId, List<ProtoField> fields, List<ProtoEnum> enums, List<Proto> nestedProtos, List<ProtoOneof> oneofs, bool nested, bool isEnum) {
            Name = name;
            CmdID = cmdId;
            Fields = fields;
            Enums = enums;
            NestedProtos = nestedProtos;
            Oneofs = oneofs;
            Nested = nested;
            IsEnum = isEnum;
        }
    }

    public class ProtoField {
        public List<ProtoType> Types;
        public string Name;
        public int FieldNumber;
        public bool IsRepeated;
        public bool IsMap;

        public ProtoField(List<ProtoType> types, string name, int fieldNumber, bool isRepeated = false, bool isMap = false) {
            Types = types;
            Name = name;
            FieldNumber = fieldNumber;
            IsRepeated = isRepeated;
            IsMap = isMap;
        }
    }

    public class ProtoType {
        public string Name;
        public bool IsImport;

        public ProtoType(string name, bool isImport = false) {
            Name = name;
            IsImport = isImport;
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
        public int FieldNumber;
        public bool IsImport;

        public ProtoOneofEntry(string type, string name, int fieldNumber, bool isImport = false) {
            Type = type;
            Name = name;
            FieldNumber = fieldNumber;
            IsImport = isImport;
        }
    }
}
