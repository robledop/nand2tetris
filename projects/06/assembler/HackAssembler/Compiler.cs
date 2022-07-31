namespace HackAssembler;

public static class Compiler
{
    static readonly Dictionary<string, string> Computation = new()
    {
        {"0", "0101010"},
        {"1", "0111111"},
        {"-1", "0111010"},
        {"D", "0001100"},
        {"A", "0110000"},
        {"M", "1110000"},
        {"!D", "0001101"},
        {"!A", "0110001"},
        {"!M", "1110001"},
        {"-D", "0001111"},
        {"-A", "0110011"},
        {"-M", "1110011"},
        {"D+1", "0011111"},
        {"A+1", "0110111"},
        {"M+1", "1110111"},
        {"D-1", "0001110"},
        {"A-1", "0110010"},
        {"M-1", "1110010"},
        {"D+A", "0000010"},
        {"D+M", "1000010"},
        {"D-A", "0010011"},
        {"D-M", "1010011"},
        {"A-D", "0000111"},
        {"M-D", "1000111"},
        {"D&A", "0000000"},
        {"D&M", "1000000"},
        {"D|A", "0010101"},
        {"D|M", "1010101"}
    };

    static readonly Dictionary<string, string> Destination = new()
    {
        {"null", "000"},
        {"M", "001"},
        {"D", "010"},
        {"MD", "011"},
        {"A", "100"},
        {"AM", "101"},
        {"AD", "110"},
        {"AMD", "111"},
    };

    static readonly Dictionary<string, string> Jump = new()
    {
        {"null", "000"},
        {"JGT", "001"},
        {"JEQ", "010"},
        {"JGE", "011"},
        {"JLT", "100"},
        {"JNE", "101"},
        {"JLE", "110"},
        {"JMP", "111"}
    };

    static readonly Dictionary<string, string> PreDefinedSymbols = new()
    {
        {"R0", "0"},
        {"R1", "1"},
        {"R2", "2"},
        {"R3", "3"},
        {"R4", "4"},
        {"R5", "5"},
        {"R6", "6"},
        {"R7", "7"},
        {"R8", "8"},
        {"R9", "9"},
        {"R10", "10"},
        {"R11", "11"},
        {"R12", "12"},
        {"R13", "13"},
        {"R14", "14"},
        {"R15", "15"},
        {"SCREEN", "16384"},
        {"KBD", "24576"},
        {"SP", "0"},
        {"LCL", "1"},
        {"ARG", "2"},
        {"THIS", "3"},
        {"THAT", "4"},
    };

    static readonly Dictionary<string, string> Labels = new();
    static readonly Dictionary<string, string> Variables = new();

    public static List<string> Compile(List<string> linesOfCode)
    {
        GenerateLabelLookUpDictionary(linesOfCode);

        var result = new List<string>();
        const int VARIABLE_START_POS = 16;
        foreach (var line in linesOfCode)
        {
            string binaryLine = "";
            // if the line starts with @, it's an A command  
            if (line.StartsWith('@'))
            {
                var argument = line.Split('@')[1];

                if (IsNumeric(argument))
                {
                    // @constant 
                    var argumentNumber = Convert.ToInt32(argument);
                    binaryLine = $"0{Convert.ToString(argumentNumber, 2).PadLeft(15, '0')}";
                }
                else
                {
                    if (PreDefinedSymbols.ContainsKey(argument))
                    {
                        //@predefined 
                        var argumentNumber = Convert.ToInt32(PreDefinedSymbols[argument]);
                        binaryLine = $"0{Convert.ToString(argumentNumber, 2).PadLeft(15, '0')}";
                    }
                    else if (Labels.ContainsKey(argument))
                    {
                        //@label
                        var argumentNumber = Convert.ToInt32(Labels[argument]);
                        binaryLine = $"0{Convert.ToString(argumentNumber, 2).PadLeft(15, '0')}";
                    }
                    else if (Variables.ContainsKey(argument))
                    {
                        //@variable for previous defined variable
                        var argumentNumber = Convert.ToInt32(Variables[argument]);
                        binaryLine = $"0{Convert.ToString(argumentNumber, 2).PadLeft(15, '0')}";
                    }
                    else
                    {
                        //@variable for new variable
                        var variablePos = VARIABLE_START_POS + Variables.Count;
                        Variables.Add(argument, variablePos.ToString());

                        var argumentNumber = Convert.ToInt32(Variables[argument]);
                        binaryLine = $"0{Convert.ToString(argumentNumber, 2).PadLeft(15, '0')}";
                    }
                }
            }
            else if (line.StartsWith('('))
            {
                // labels were already processed and are ignored here
                continue;
            }
            else // C command
            {
                binaryLine += "111";                    // C command opcode
                if (line.Contains('='))
                {
                    var dest = line.Split('=')[0];      //destination
                    var comp = line.Split('=', ';')[1]; //computation

                    binaryLine += Computation[comp];
                    binaryLine += Destination[dest];

                    // if the line contains a jump, add it to the binary line
                    if (line.Contains(';'))
                    {
                        var jump = line.Split(';')[1];
                        binaryLine += Jump[jump];
                    }
                    else
                    {
                        // if the line doesn't contain a jump, add a null jump
                        binaryLine += "000";
                    }
                }
                else
                {
                    // if the line doesn't contain an equals sign, it's a C command without a destination
                    var comp = line.Split(";")[0];
                    binaryLine += Computation[comp];
                    binaryLine += "000"; // null destination
                    if (line.Contains(';'))
                    {
                        // if the line contains a jump, add it to the binary line
                        var jump = line.Split(';')[1];
                        binaryLine += Jump[jump];
                    }
                    else
                    {
                        // if the line doesn't contain a jump, add a null jump
                        binaryLine += "000";
                    }

                }
            }

            result.Add(binaryLine);
        }

        return result;
    }

    static void GenerateLabelLookUpDictionary(List<string> linesOfCode)
    {
        var lineNumber = 0;
        foreach (var line in linesOfCode)
        {
            // if the line starts with '(', it's a label, add it to the dictionary
            if (line.StartsWith('('))
            {
                var label = line.Split('(', ')')[1];
                Labels.Add(label, (lineNumber).ToString());
            }
            else
            {
                lineNumber++;
            }
        }
    }

    static bool IsNumeric(string value)
    {
        return value.All(char.IsNumber);
    }
}