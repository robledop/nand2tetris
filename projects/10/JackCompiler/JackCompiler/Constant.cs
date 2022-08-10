namespace JackCompiler
{
    public class Constant
    {
        public string? String { get; set; }
        public int Int { get; set; }
        public ConstantType Type { get; set; }

        public Constant(string stringConstant)
        {
            String = stringConstant;
            Type = ConstantType.String;
        }

        public Constant(int integerConstant)
        {
            Int = integerConstant;
            Type = ConstantType.Int;
        }
    }


    public enum ConstantType
    {
        Int,
        String
    }
}