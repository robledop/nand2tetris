using System.Xml;
using FluentAssertions;
using JackCompiler.JackAnalyzer;
using JackCompiler.JackCodeGenerator;

namespace JackCompiler.Tests;

public class CodeGeneratorTests
{


	[Fact]
	public void Square_GenerateClassSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		codeGenerator.GenerateClassSymbolTable();

		codeGenerator.ClassSymbolTable.Count.Should().Be(3);

		var compareTo = new List<Symbol>
		{
			new Symbol("x", "int", SymbolKind.Field, 0),
			new Symbol("y", "int", SymbolKind.Field, 1),
			new Symbol("size", "int", SymbolKind.Field, 2),
		};

		codeGenerator.ClassSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void Square_GenerateSubroutineSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var constructor = parseTree.SelectSingleNode("//subroutineDec[identifier=' new ']");

		codeGenerator.CompileSubroutine(constructor);


		var compareTo = new List<Symbol>
		{
			new Symbol("Ax", "int", SymbolKind.Argument, 0),
			new Symbol("Ay", "int", SymbolKind.Argument, 1),
			new Symbol("Asize", "int", SymbolKind.Argument, 2)
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void SquareMain_GenerateClassSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");

		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.GenerateClassSymbolTable();

		codeGenerator.ClassSymbolTable.Count.Should().Be(1);

		var compareTo = new List<Symbol>
		{
			new Symbol("test", "boolean", SymbolKind.Static, 0)
		};

		codeGenerator.ClassSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void SquareMain_GenerateSubroutineSymbolTable_more()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		var functionMore = parseTree.SelectSingleNode("//subroutineDec[identifier=' more ']");

		codeGenerator.CompileSubroutine(functionMore);

		var compareTo = new List<Symbol>
		{
			new Symbol("i", "int", SymbolKind.Local, 0),
			new Symbol("j", "int", SymbolKind.Local, 1),
			new Symbol("s", "String", SymbolKind.Local, 2),
			new Symbol("a", "Array", SymbolKind.Local, 3),
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void Square_GenerateSubroutineSymbolTable_draw()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		var functionMore = parseTree.SelectSingleNode("//subroutineDec[identifier=' draw ']");
		codeGenerator.CompileClass();
		codeGenerator.CompileSubroutine(functionMore);

		codeGenerator.SubroutineSymbolTable.Count.Should().Be(1);

		var compareTo = new List<Symbol>
		{
			new Symbol("this", "Square", SymbolKind.Argument, 0),
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void ArrayTestMain_GenerateClassSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");

		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		codeGenerator.GenerateClassSymbolTable();

		codeGenerator.ClassSymbolTable.Count.Should().Be(0);
	}

	[Fact]
	public void ArrayTestMain_GenerateSubroutineSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");

		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var functionMain = parseTree.SelectSingleNode("//subroutineDec[identifier=' main ']");

		codeGenerator.CompileSubroutine(functionMain);

		var compareTo = new List<Symbol>
		{
			new Symbol("a", "Array", SymbolKind.Local, 0),
			new Symbol("length", "int", SymbolKind.Local, 1),
			new Symbol("i", "int", SymbolKind.Local, 2),
			new Symbol("sum", "int", SymbolKind.Local, 3),
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void CompileExpression_IntegerConstant()
	{
		var expressionNodeXml = @"
			<expression>
				<term>
					<integerConstant> 10 </integerConstant>
				</term>
			</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml);

		var codeGenerator = new CodeGenerator(xmlDocument);

		var vmCode = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode.Should().Be("push constant 10");
	}

	[Fact]
	public void CompileExpression_Argument()
	{
		var source = @"
			class Square {
				field int x, y, size;
			   constructor Square new(int Ax, int Ay, int Asize) {
				  let x = Ax;
				  let y = Ay;
				  let size = Asize;
				  return this;
			   }
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
		   <expression>
			<term>
			  <identifier> Ax </identifier>
			</term>
		  </expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be("push argument 0");

		var expressionNodeXml2 = @"
			<expression>
				<term>
					<identifier> Ay </identifier>
				</term>
			</expression>";
		xmlDocument.LoadXml(expressionNodeXml2);

		var vmCode2 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode2.Should().Be("push argument 1");
	}

	[Fact]
	public void CompileExpression_UnaryOp()
	{
		var source = @"
			class Main {
				function void more() { 
					var int i, j;      
					let i = i * (-j);
					return;
				}
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
		   <expression>
				<term>
					<symbol> - </symbol>
					<term>
						<identifier> j </identifier>
					</term>
				</term>
		   </expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode.Should().Be(
@"push local 1
neg");
	}

	[Fact]
	public void CompileExpression_Op()
	{
		var source = @"
			class Main {
				function void more() { 
					var int i, j;      
					let i = i | j;
					let i = i * j;
					return;
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
			<expression>
				<term>
					<identifier> i </identifier>
				</term>
				<symbol> | </symbol>
				<term>
					<identifier> j </identifier>
				</term>
			</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push local 0
push local 1
or");

		var expressionNodeXml2 = @"
			<expression>
				<term>
					<identifier> i </identifier>
				</term>
				<symbol> * </symbol>
				<term>
					<identifier> j </identifier>
				</term>
			</expression>";

		xmlDocument.LoadXml(expressionNodeXml2);
		var vmCode2 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();
		vmCode2.Should().Be(
@"push local 0
push local 1
call Math.multiply 2");
	}

	[Fact]
	public void CompileExpression_ExpressionListEmpty()
	{
		var source = @"
			class Cell 
			{
				field Array _neighbors;

				method void determineNextLiveState()
				{
					var Cell currentNeighbor;
					var int liveNeighbors, i;
					
					let liveNeighbors = 0;
					let i = 0;
					while(i < 8)
					{
						let currentNeighbor = _neighbors[i];
						if(currentNeighbor.getIsAlive())
						{
							let liveNeighbors = liveNeighbors + 1;
						}
						let i = i + 1;
					}
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
				<expression>
					<term>
					  <identifier> currentNeighbor </identifier>
					  <symbol> . </symbol>
					  <identifier> getIsAlive </identifier>
					  <symbol> ( </symbol>
					  <expressionList>
					  </expressionList>
					  <symbol> ) </symbol>
					</term>
				</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push local 0
call Cell.getIsAlive 1");
	}

	[Fact]
	public void CompileExpression_ExpressionList()
	{
		var source = @"
			class Cell 
			{
				method void determineNextLiveState()
				{
					var Cell currentNeighbor;
					var int a;
					let a = 10;
					if(currentNeighbor.getIsAlive(1, a))
					{
					}
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
				<expression>
					<term>
					  <identifier> currentNeighbor </identifier>
					  <symbol> . </symbol>
					  <identifier> getIsAlive </identifier>
					  <symbol> ( </symbol>
					  <expressionList>
						  <expression>
							<term>
								<integerConstant> 1 </integerConstant>
							</term>
						  </expression>
						  <symbol> , </symbol>
						  <expression>
							<term>
								<identifier> a </identifier>
							</term>
						  </expression>
					  </expressionList>
					  <symbol> ) </symbol>
					</term>
				</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push local 0
push constant 1
push local 1
call Cell.getIsAlive 3");
	}

	[Fact]
	public void CompileLetStatements()
	{
		var source = @"
			class Cell {
			  method void determineNextLiveState() {
				var Cell currentNeighbor;
				var int liveNeighbors, i;
				let liveNeighbors = 0;
				let i = 0;
				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<letStatement>
			  <keyword> let </keyword>
			  <identifier> liveNeighbors </identifier>
			  <symbol> = </symbol>
			  <expression>
				<term>
				  <integerConstant> 0 </integerConstant>
				</term>
			  </expression>
			  <symbol> ; </symbol>
			</letStatement>
			<letStatement>
			  <keyword> let </keyword>
			  <identifier> i </identifier>
			  <symbol> = </symbol>
			  <expression>
				<term>
				  <integerConstant> 0 </integerConstant>
				</term>
			  </expression>
			  <symbol> ; </symbol>
			</letStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
			@"push constant 0
pop local 1
push constant 0
pop local 2");
	}

	[Fact]
	public void CompileWhileStatements()
	{
		var source = @"
			class Cell {
			   method void determineNextLiveState() {
				var int liveNeighbors, i;
					
				let i = 0;
				while(i < 8)
				{
				  let i = i + 1;
				}

				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<whileStatement>
			  <keyword> while </keyword>
			  <symbol> ( </symbol>
			  <expression>
				<term>
				  <identifier> i </identifier>
				</term>
				<symbol> &lt; </symbol>
				<term>
				  <integerConstant> 8 </integerConstant>
				</term>
			  </expression>
			  <symbol> ) </symbol>
			  <symbol> { </symbol>
			  <statements>
				<letStatement>
				  <keyword> let </keyword>
				  <identifier> i </identifier>
				  <symbol> = </symbol>
				  <expression>
					<term>
					  <identifier> i </identifier>
					</term>
					<symbol> + </symbol>
					<term>
					  <integerConstant> 1 </integerConstant>
					</term>
				  </expression>
				  <symbol> ; </symbol>
				</letStatement>
			  </statements>
			  <symbol> } </symbol>
			</whileStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
			@"label WHILE_EXP1
push local 1
push constant 8
lt
not
if-goto WHILE_END1
push local 1
push constant 1
add
pop local 1
goto WHILE_EXP1
label WHILE_END1");
	}

	[Fact]
	public void CompileIfStatements()
	{
		var source = @"
			class Cell {
			   method void determineNextLiveState() {
				var int liveNeighbors, i;
					
				let i = 0;
				while(i < 8)
				{
				  if ( i < 4 ) {
					let i = i + 1;
				  } else {
					let i = 1 + 1;
				  }
				  let i = i + 1;
				}

				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<ifStatement>
			  <keyword> if </keyword>
			  <symbol> ( </symbol>
			  <expression>
				<term>
				  <identifier> i </identifier>
				</term>
				<symbol> &lt; </symbol>
				<term>
				  <integerConstant> 8 </integerConstant>
				</term>
			  </expression>
			  <symbol> ) </symbol>
			  <symbol> { </symbol>
			  <statements>
				<letStatement>
				  <keyword> let </keyword>
				  <identifier> i </identifier>
				  <symbol> = </symbol>
				  <expression>
					<term>
					  <identifier> i </identifier>
					</term>
					<symbol> + </symbol>
					<term>
					  <integerConstant> 1 </integerConstant>
					</term>
				  </expression>
				  <symbol> ; </symbol>
				</letStatement>
			  </statements>
			  <symbol> } </symbol>
			  <keyword> else </keyword>
			  <symbol> { </symbol>
			  <statements>
				<letStatement>
				  <keyword> let </keyword>
				  <identifier> i </identifier>
				  <symbol> = </symbol>
				  <expression>
					<term>
					  <identifier> i </identifier>
					</term>
					<symbol> + </symbol>
					<term>
					  <integerConstant> 1 </integerConstant>
					</term>
				  </expression>
				  <symbol> ; </symbol>
				</letStatement>
			  </statements>
			  <symbol> } </symbol>
			</ifStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
			@"push local 1
push constant 8
lt
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push local 1
push constant 1
add
pop local 1
goto IF_END1
label IF_FALSE1
push local 1
push constant 1
add
pop local 1
label IF_END1");
	}

	[Fact]
	public void CompileDoStatements()
	{
		var source = @"
			class Cell {
			  field Array _neighbors;
			  field Cell _cell;

			  method void test()   {
				do Memory.deAlloc(_neighbors);
				do _cell.advance(1);
				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<doStatement>
			  <keyword> do </keyword>
			  <identifier> Memory </identifier>
			  <symbol> . </symbol>
			  <identifier> deAlloc </identifier>
			  <symbol> ( </symbol>
			  <expressionList>
				<expression>
				  <term>
					<identifier> _neighbors </identifier>
				  </term>
				</expression>
			  </expressionList>
			  <symbol> ) </symbol>
			  <symbol> ; </symbol>
			</doStatement>
			<doStatement>
			  <keyword> do </keyword>
			  <identifier> _cell </identifier>
			  <symbol> . </symbol>
			  <identifier> advance </identifier>
			  <symbol> ( </symbol>
			  <expressionList>
				<expression>
				  <term>
					<integerConstant> 1 </integerConstant>
				  </term>
				</expression>
			  </expressionList>
			  <symbol> ) </symbol>
			  <symbol> ; </symbol>
			</doStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push this 0
call Memory.deAlloc 1
pop temp 0
push this 1
push constant 1
call Cell.advance 2
pop temp 0");
	}

	[Fact]
	public void CompileSeven()
	{
		var source = @"
			class Main {
			   function void main() {
				  do Output.printInt(1 + (2 * 3));
				  return;
			   }
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
@"function Main.main 0
push constant 1
push constant 2
push constant 3
call Math.multiply 2
add
call Output.printInt 1
pop temp 0
push constant 0
return
");
	}

	[Fact]
	public void CompileConvertToBin()
	{
		var source = @"
		   class Main {
			function void main() {
				var int value;
				do Main.fillMemory(8001, 16, -1); // sets RAM[8001]..RAM[8016] to -1
				let value = Memory.peek(8000);    // reads a value from RAM[8000]
				do Main.convert(value);           // performs the conversion
				return;
			}
			
			function void convert(int value) {
				var int mask, position;
				var boolean loop;
				
				let loop = true;
				while (loop) {
					let position = position + 1;
					let mask = Main.nextMask(mask);
				
					if (~(position > 16)) {
				
						if (~((value & mask) = 0)) {
							do Memory.poke(8000 + position, 1);
						}
						else {
							do Memory.poke(8000 + position, 0);
						}    
					}
					else {
						let loop = false;
					}
				}
				return;
			}
		 
			function int nextMask(int mask) {
				if (mask = 0) {
					return 1;
				}
				else {
				return mask * 2;
				}
			}
			
			function void fillMemory(int startAddress, int length, int value) {
				while (length > 0) {
					do Memory.poke(startAddress, value);
					let length = length - 1;
					let startAddress = startAddress + 1;
				}
				return;
			}
		}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
@"function Main.main 1
push constant 8001
push constant 16
push constant 1
neg
call Main.fillMemory 3
pop temp 0
push constant 8000
call Memory.peek 1
pop local 0
push local 0
call Main.convert 1
pop temp 0
push constant 0
return
function Main.convert 3
push constant 0
not
pop local 2
label WHILE_EXP0
push local 2
not
if-goto WHILE_END0
push local 1
push constant 1
add
pop local 1
push local 0
call Main.nextMask 1
pop local 0
push local 1
push constant 16
gt
not
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push argument 0
push local 0
and
push constant 0
eq
not
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 8000
push local 1
add
push constant 1
call Memory.poke 2
pop temp 0
goto IF_END1
label IF_FALSE1
push constant 8000
push local 1
add
push constant 0
call Memory.poke 2
pop temp 0
label IF_END1
goto IF_END0
label IF_FALSE0
push constant 0
pop local 2
label IF_END0
goto WHILE_EXP0
label WHILE_END0
push constant 0
return
function Main.nextMask 0
push argument 0
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 1
return
goto IF_END0
label IF_FALSE0
push argument 0
push constant 2
call Math.multiply 2
return
label IF_END0
function Main.fillMemory 0
label WHILE_EXP0
push argument 1
push constant 0
gt
not
if-goto WHILE_END0
push argument 0
push argument 2
call Memory.poke 2
pop temp 0
push argument 1
push constant 1
sub
pop argument 1
push argument 0
push constant 1
add
pop argument 0
goto WHILE_EXP0
label WHILE_END0
push constant 0
return
");
	}

	[Fact]
	public void CompileAverage()
	{
		var source = @"
			class Main {
			   function void main() {
				 var Array a; 
				 var int length;
				 var int i, sum;

				 let length = Keyboard.readInt(""How many numbers? "");
				 let a = Array.new(length); // constructs the array
				 
				 let i = 0;
				 while (i < length) {
					let a[i] = Keyboard.readInt(""Enter a number: "");
					let sum = sum + a[i];
					let i = i + 1;
				 }
				 
				 do Output.printString(""The average is "");
				 do Output.printInt(sum / length);
				 return;
			   }
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
@"function Main.main 4
push constant 18
call String.new 1
push constant 72
call String.appendChar 2
push constant 111
call String.appendChar 2
push constant 119
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 109
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 110
call String.appendChar 2
push constant 121
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 110
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 109
call String.appendChar 2
push constant 98
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 63
call String.appendChar 2
push constant 32
call String.appendChar 2
call Keyboard.readInt 1
pop local 1
push local 1
call Array.new 1
pop local 0
push constant 0
pop local 2
label WHILE_EXP0
push local 2
push local 1
lt
not
if-goto WHILE_END0
push local 2
push local 0
add
push constant 16
call String.new 1
push constant 69
call String.appendChar 2
push constant 110
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 110
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 109
call String.appendChar 2
push constant 98
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Keyboard.readInt 1
pop temp 0
pop pointer 1
push temp 0
pop that 0
push local 3
push local 2
push local 0
add
pop pointer 1
push that 0
add
pop local 3
push local 2
push constant 1
add
pop local 2
goto WHILE_EXP0
label WHILE_END0
push constant 15
call String.new 1
push constant 84
call String.appendChar 2
push constant 104
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 118
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 103
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 105
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push local 3
push local 1
call Math.divide 2
call Output.printInt 1
pop temp 0
push constant 0
return
");
	}

	[Fact]
	public void CompileSquareMain()
	{
		var source = @"
			class Main {
				function void main() {
					var SquareGame game;
					let game = SquareGame.new();
					do game.run();
					do game.dispose();
					return;
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function Main.main 1
call SquareGame.new 0
pop local 0
push local 0
call SquareGame.run 1
pop temp 0
push local 0
call SquareGame.dispose 1
pop temp 0
push constant 0
return
");
	}

	[Fact]
	public void CompileSquareSquare()
	{
		var source = @"
			class Square {
			   field int x, y; // screen location of the square's top-left corner
			   field int size; // length of this square, in pixels

			   /** Constructs a new square with a given location and size. */
			   constructor Square new(int Ax, int Ay, int Asize) {
				  let x = Ax;
				  let y = Ay;
				  let size = Asize;
				  do draw();
				  return this;
			   }

			   /** Disposes this square. */
			   method void dispose() {
				  do Memory.deAlloc(this);
				  return;
			   }

			   /** Draws the square on the screen. */
			   method void draw() {
				  do Screen.setColor(true);
				  do Screen.drawRectangle(x, y, x + size, y + size);
				  return;
			   }

			   /** Erases the square from the screen. */
			   method void erase() {
				  do Screen.setColor(false);
				  do Screen.drawRectangle(x, y, x + size, y + size);
				  return;
			   }

				/** Increments the square size by 2 pixels. */
			   method void incSize() {
				  if (((y + size) < 254) & ((x + size) < 510)) {
					 do erase();
					 let size = size + 2;
					 do draw();
				  }
				  return;
			   }

			   /** Decrements the square size by 2 pixels. */
			   method void decSize() {
				  if (size > 2) {
					 do erase();
					 let size = size - 2;
					 do draw();
				  }
				  return;
			   }

			   /** Moves the square up by 2 pixels. */
			   method void moveUp() {
				  if (y > 1) {
					 do Screen.setColor(false);
					 do Screen.drawRectangle(x, (y + size) - 1, x + size, y + size);
					 let y = y - 2;
					 do Screen.setColor(true);
					 do Screen.drawRectangle(x, y, x + size, y + 1);
				  }
				  return;
			   }

			   /** Moves the square down by 2 pixels. */
			   method void moveDown() {
				  if ((y + size) < 254) {
					 do Screen.setColor(false);
					 do Screen.drawRectangle(x, y, x + size, y + 1);
					 let y = y + 2;
					 do Screen.setColor(true);
					 do Screen.drawRectangle(x, (y + size) - 1, x + size, y + size);
				  }
				  return;
			   }

			   /** Moves the square left by 2 pixels. */
			   method void moveLeft() {
											  if (x > 1) {
					 do Screen.setColor(false);
					 do Screen.drawRectangle((x + size) - 1, y, x + size, y + size);
					 let x = x - 2;
					 do Screen.setColor(true);
					 do Screen.drawRectangle(x, y, x + 1, y + size);
				  }
				  return;
			   }

			   /** Moves the square right by 2 pixels. */
			   method void moveRight() {
				  if ((x + size) < 510) {
					 do Screen.setColor(false);
					 do Screen.drawRectangle(x, y, x + 1, y + size);
					 let x = x + 2;
					 do Screen.setColor(true);
					 do Screen.drawRectangle((x + size) - 1, y, x + size, y + size);
				  }
				  return;
			   }
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function Square.new 0
push constant 3
call Memory.alloc 1
pop pointer 0
push argument 0
pop this 0
push argument 1
pop this 1
push argument 2
pop this 2
push pointer 0
call Square.draw 1
pop temp 0
push pointer 0
return
function Square.dispose 0
push argument 0
pop pointer 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function Square.draw 0
push argument 0
pop pointer 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Square.erase 0
push argument 0
pop pointer 0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Square.incSize 0
push argument 0
pop pointer 0
push this 1
push this 2
add
push constant 254
lt
push this 0
push this 2
add
push constant 510
lt
and
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push pointer 0
call Square.erase 1
pop temp 0
push this 2
push constant 2
add
pop this 2
push pointer 0
call Square.draw 1
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.decSize 0
push argument 0
pop pointer 0
push this 2
push constant 2
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push pointer 0
call Square.erase 1
pop temp 0
push this 2
push constant 2
sub
pop this 2
push pointer 0
call Square.draw 1
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveUp 0
push argument 0
pop pointer 0
push this 1
push constant 1
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 2
add
push constant 1
sub
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push this 1
push constant 2
sub
pop this 1
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push constant 1
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveDown 0
push argument 0
pop pointer 0
push this 1
push this 2
add
push constant 254
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push constant 1
add
call Screen.drawRectangle 4
pop temp 0
push this 1
push constant 2
add
pop this 1
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 2
add
push constant 1
sub
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveLeft 0
push argument 0
pop pointer 0
push this 0
push constant 1
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 2
add
push constant 1
sub
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push this 0
push constant 2
sub
pop this 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push constant 1
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveRight 0
push argument 0
pop pointer 0
push this 0
push this 2
add
push constant 510
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push constant 1
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push this 0
push constant 2
add
pop this 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 2
add
push constant 1
sub
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
");
	}

	[Fact]
	public void CompileSquareSquareGame()
	{
		var source = @"
			class SquareGame {
   field Square square; // the square of this game
   field int direction; // the square's current direction: 
						// 0=none, 1=up, 2=down, 3=left, 4=right

   /** Constructs a new Square Game. */
   constructor SquareGame new() {
	  // Creates a 30 by 30 pixels square and positions it at the top-left
	  // of the screen.
	  let square = Square.new(0, 0, 30);
	  let direction = 0;  // initial state is no movement
	  return this;
   }

   /** Disposes this game. */
   method void dispose() {
	  do square.dispose();
	  do Memory.deAlloc(this);
	  return;
   }

   /** Moves the square in the current direction. */
   method void moveSquare() {
	  if (direction = 1) { do square.moveUp(); }
	  if (direction = 2) { do square.moveDown(); }
	  if (direction = 3) { do square.moveLeft(); }
	  if (direction = 4) { do square.moveRight(); }
	  do Sys.wait(5);  // delays the next movement
	  return;
   }

   /** Runs the game: handles the user's inputs and moves the square accordingly */
   method void run() {
	  var char key;  // the key currently pressed by the user
	  var boolean exit;
	  let exit = false;
	  
	  while (~exit) {
		 // waits for a key to be pressed
		 while (key = 0) {
			let key = Keyboard.keyPressed();
			do moveSquare();
		 }
		 if (key = 81)  { let exit = true; }     // q key
		 if (key = 90)  { do square.decSize(); } // z key
		 if (key = 88)  { do square.incSize(); } // x key
		 if (key = 131) { let direction = 1; }   // up arrow
		 if (key = 133) { let direction = 2; }   // down arrow
		 if (key = 130) { let direction = 3; }   // left arrow
		 if (key = 132) { let direction = 4; }   // right arrow

		 // waits for the key to be released
		 while (~(key = 0)) {
			let key = Keyboard.keyPressed();
			do moveSquare();
		 }
	 } // while
	 return;
   }
}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function SquareGame.new 0
push constant 2
call Memory.alloc 1
pop pointer 0
push constant 0
push constant 0
push constant 30
call Square.new 3
pop this 0
push constant 0
pop this 1
push pointer 0
return
function SquareGame.dispose 0
push argument 0
pop pointer 0
push this 0
call Square.dispose 1
pop temp 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function SquareGame.moveSquare 0
push argument 0
pop pointer 0
push this 1
push constant 1
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 0
call Square.moveUp 1
pop temp 0
label IF_FALSE0
push this 1
push constant 2
eq
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 0
call Square.moveDown 1
pop temp 0
label IF_FALSE1
push this 1
push constant 3
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push this 0
call Square.moveLeft 1
pop temp 0
label IF_FALSE2
push this 1
push constant 4
eq
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push this 0
call Square.moveRight 1
pop temp 0
label IF_FALSE3
push constant 5
call Sys.wait 1
pop temp 0
push constant 0
return
function SquareGame.run 2
push argument 0
pop pointer 0
push constant 0
pop local 1
label WHILE_EXP0
push local 1
not
not
if-goto WHILE_END0
label WHILE_EXP1
push local 0
push constant 0
eq
not
if-goto WHILE_END1
call Keyboard.keyPressed 0
pop local 0
push pointer 0
call SquareGame.moveSquare 1
pop temp 0
goto WHILE_EXP1
label WHILE_END1
push local 0
push constant 81
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
not
pop local 1
label IF_FALSE0
push local 0
push constant 90
eq
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 0
call Square.decSize 1
pop temp 0
label IF_FALSE1
push local 0
push constant 88
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push this 0
call Square.incSize 1
pop temp 0
label IF_FALSE2
push local 0
push constant 131
eq
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push constant 1
pop this 1
label IF_FALSE3
push local 0
push constant 133
eq
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push constant 2
pop this 1
label IF_FALSE4
push local 0
push constant 130
eq
if-goto IF_TRUE5
goto IF_FALSE5
label IF_TRUE5
push constant 3
pop this 1
label IF_FALSE5
push local 0
push constant 132
eq
if-goto IF_TRUE6
goto IF_FALSE6
label IF_TRUE6
push constant 4
pop this 1
label IF_FALSE6
label WHILE_EXP2
push local 0
push constant 0
eq
not
not
if-goto WHILE_END2
call Keyboard.keyPressed 0
pop local 0
push pointer 0
call SquareGame.moveSquare 1
pop temp 0
goto WHILE_EXP2
label WHILE_END2
goto WHILE_EXP0
label WHILE_END0
push constant 0
return
");
	}

	[Fact]
	public void CompileComplexArrays()
	{
		var source = @"
			class Main {
				function void main() {
					var Array a, b, c;
					
					let a = Array.new(10);
					let b = Array.new(5);
					let c = Array.new(1);
					
					let a[3] = 2;
					let a[4] = 8;
					let a[5] = 4;
					let b[a[3]] = a[3] + 3;  // b[2] = 5
					let a[b[a[3]]] = a[a[5]] * b[((7 - a[3]) - Main.double(2)) + 1];  // a[5] = 8 * 5 = 40
					let c[0] = null;
					let c = c[0];
					
					do Output.printString(""Test 1: expected result: 5; actual result: "");
					do Output.printInt(b[2]);
					do Output.println();
					do Output.printString(""Test 2: expected result: 40; actual result: "");
					do Output.printInt(a[5]);
					do Output.println();
					do Output.printString(""Test 3: expected result: 0; actual result: "");
					do Output.printInt(c);
					do Output.println();
					
					let c = null;

					if (c = null) {
						do Main.fill(a, 10);
						let c = a[3];
						let c[1] = 33;
						let c = a[7];
						let c[1] = 77;
						let b = a[3];
						let b[1] = b[1] + c[1];  // b[1] = 33 + 77 = 110;
					}

					do Output.printString(""Test 4: expected result: 77; actual result: "");
					do Output.printInt(c[1]);
					do Output.println();
					do Output.printString(""Test 5: expected result: 110; actual result: "");
					do Output.printInt(b[1]);
					do Output.println();
					return;
				}
				
				function int double(int a) {
					return a * 2;
				}
				
				function void fill(Array a, int size) {
					while (size > 0) {
						let size = size - 1;
						let a[size] = Array.new(3);
					}
					return;
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function Main.main 3
push constant 10
call Array.new 1
pop local 0
push constant 5
call Array.new 1
pop local 1
push constant 1
call Array.new 1
pop local 2
push constant 3
push local 0
add
push constant 2
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 4
push local 0
add
push constant 8
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 5
push local 0
add
push constant 4
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push local 0
add
pop pointer 1
push that 0
push local 1
add
push constant 3
push local 0
add
pop pointer 1
push that 0
push constant 3
add
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push local 0
add
pop pointer 1
push that 0
push local 1
add
pop pointer 1
push that 0
push local 0
add
push constant 5
push local 0
add
pop pointer 1
push that 0
push local 0
add
pop pointer 1
push that 0
push constant 7
push constant 3
push local 0
add
pop pointer 1
push that 0
sub
push constant 2
call Main.double 1
sub
push constant 1
add
push local 1
add
pop pointer 1
push that 0
call Math.multiply 2
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 0
push local 2
add
push constant 0
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 0
push local 2
add
pop pointer 1
push that 0
pop local 2
push constant 43
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 49
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 53
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 2
push local 1
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 44
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 50
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 52
call String.appendChar 2
push constant 48
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 5
push local 0
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 43
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 51
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 48
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push local 2
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 0
pop local 2
push local 2
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push local 0
push constant 10
call Main.fill 2
pop temp 0
push constant 3
push local 0
add
pop pointer 1
push that 0
pop local 2
push constant 1
push local 2
add
push constant 33
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 7
push local 0
add
pop pointer 1
push that 0
pop local 2
push constant 1
push local 2
add
push constant 77
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push local 0
add
pop pointer 1
push that 0
pop local 1
push constant 1
push local 1
add
push constant 1
push local 1
add
pop pointer 1
push that 0
push constant 1
push local 2
add
pop pointer 1
push that 0
add
pop temp 0
pop pointer 1
push temp 0
pop that 0
label IF_FALSE0
push constant 44
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 52
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 55
call String.appendChar 2
push constant 55
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 1
push local 2
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 45
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 53
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 49
call String.appendChar 2
push constant 49
call String.appendChar 2
push constant 48
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 1
push local 1
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 0
return
function Main.double 0
push argument 0
push constant 2
call Math.multiply 2
return
function Main.fill 0
label WHILE_EXP0
push argument 1
push constant 0
gt
not
if-goto WHILE_END0
push argument 1
push constant 1
sub
pop argument 1
push argument 1
push argument 0
add
push constant 3
call Array.new 1
pop temp 0
pop pointer 1
push temp 0
pop that 0
goto WHILE_EXP0
label WHILE_END0
push constant 0
return
");
	}

	[Fact]
	public void CompilePongBall()
	{
		var source = @"
			class Ball {

				field int x, y;               // the ball's screen location (in pixels)
				field int lengthx, lengthy;   // distance of last destination (in pixels)

				field int d, straightD, diagonalD;            // used for straight line movement computation
				field boolean invert, positivex, positivey;   // (same)
			   
				field int leftWall, rightWall, topWall, bottomWall;  // wall locations
			   
				field int wall;   // last wall that the ball was bounced off of

				/** Constructs a new ball with the given initial location and wall locations. */
				constructor Ball new(int Ax, int Ay,
									 int AleftWall, int ArightWall, int AtopWall, int AbottomWall) {    	
					let x = Ax;		
					let y = Ay;
					let leftWall = AleftWall;
					let rightWall = ArightWall - 6;    // -6 for ball size
					let topWall = AtopWall; 
					let bottomWall = AbottomWall - 6;  // -6 for ball size
					let wall = 0;
					do show();
					return this;
				}

				/** Deallocates the Ball's memory. */
				method void dispose() {
					do Memory.deAlloc(this);
					return;
				}

				/** Shows the ball. */
				method void show() {
					do Screen.setColor(true);
					do draw();
					return;
				}

				/** Hides the ball. */
				method void hide() {
					do Screen.setColor(false);
					do draw();
					return;
				}

				/** Draws the ball. */
				method void draw() {
					do Screen.drawRectangle(x, y, x + 5, y + 5);
					return;
				}

				/** Returns the ball's left edge. */
				method int getLeft() {
					return x;
				}

				/** Returns the ball's right edge. */
				method int getRight() {
					return x + 5;
				}

				/** Computes and sets the ball's destination. */
				method void setDestination(int destx, int desty) {
					var int dx, dy, temp;
					let lengthx = destx - x;
					let lengthy = desty - y;
					let dx = Math.abs(lengthx);
					let dy = Math.abs(lengthy);
					let invert = (dx < dy);

					if (invert) {
						let temp = dx; // swap dx, dy
						let dx = dy;
						let dy = temp;
						let positivex = (y < desty);
						let positivey = (x < destx);
					}
					else {
						let positivex = (x < destx);
						let positivey = (y < desty);
					}

					let d = (2 * dy) - dx;
					let straightD = 2 * dy;
					let diagonalD = 2 * (dy - dx);

					return;
				}

				/**
				 * Moves the ball one unit towards its destination.
				 * If the ball has reached a wall, returns 0.
				 * Else, returns a value according to the wall:
				 * 1 (left wall), 2 (right wall), 3 (top wall), 4 (bottom wall).
				 */
				method int move() {

					do hide();

					if (d < 0) { let d = d + straightD; }
					else {
						let d = d + diagonalD;

						if (positivey) {
							if (invert) { let x = x + 4; }
							else { let y = y + 4; }
						}
						else {
							if (invert) { let x = x - 4; }
							else { let y = y - 4; }
						}
					}

					if (positivex) {
						if (invert) { let y = y + 4; }
						else { let x = x + 4; }
					}
					else {
						if (invert) { let y = y - 4; }
						else { let x = x - 4; }
					}

					if (~(x > leftWall)) {
						let wall = 1;    
						let x = leftWall;
					}
					if (~(x < rightWall)) {
						let wall = 2;    
						let x = rightWall;
					}
					if (~(y > topWall)) {
						let wall = 3;    
						let y = topWall;
					}
					if (~(y < bottomWall)) {
						let wall = 4;    
						let y = bottomWall;
					}

					do show();

					return wall;
				}

				/**
				 * Bounces off the current wall: sets the new destination
				 * of the ball according to the ball's angle and the given
				 * bouncing direction (-1/0/1=left/center/right or up/center/down).
				 */
				method void bounce(int bouncingDirection) {
					var int newx, newy, divLengthx, divLengthy, factor;

					// dividing by 10 first since results are too big
					let divLengthx = lengthx / 10;
					let divLengthy = lengthy / 10;
					if (bouncingDirection = 0) { let factor = 10; }
					else {
						if (((~(lengthx < 0)) & (bouncingDirection = 1)) | ((lengthx < 0) & (bouncingDirection = (-1)))) {
							let factor = 20; // bounce direction is in ball direction
						}
						else { let factor = 5; } // bounce direction is against ball direction
					}

					if (wall = 1) {
						let newx = 506;
						let newy = (divLengthy * (-50)) / divLengthx;
						let newy = y + (newy * factor);
					}
					else {
						if (wall = 2) {
							let newx = 0;
							let newy = (divLengthy * 50) / divLengthx;
							let newy = y + (newy * factor);
						}
						else {
							if (wall = 3) {
								let newy = 250;
								let newx = (divLengthx * (-25)) / divLengthy;
								let newx = x + (newx * factor);
							}
							else { // assumes wall = 4
								let newy = 0;
								let newx = (divLengthx * 25) / divLengthy;
								let newx = x + (newx * factor);
							}
						}
					}

					do setDestination(newx, newy);
					return;
				}
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function Ball.new 0
push constant 15
call Memory.alloc 1
pop pointer 0
push argument 0
pop this 0
push argument 1
pop this 1
push argument 2
pop this 10
push argument 3
push constant 6
sub
pop this 11
push argument 4
pop this 12
push argument 5
push constant 6
sub
pop this 13
push constant 0
pop this 14
push pointer 0
call Ball.show 1
pop temp 0
push pointer 0
return
function Ball.dispose 0
push argument 0
pop pointer 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function Ball.show 0
push argument 0
pop pointer 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push pointer 0
call Ball.draw 1
pop temp 0
push constant 0
return
function Ball.hide 0
push argument 0
pop pointer 0
push constant 0
call Screen.setColor 1
pop temp 0
push pointer 0
call Ball.draw 1
pop temp 0
push constant 0
return
function Ball.draw 0
push argument 0
pop pointer 0
push this 0
push this 1
push this 0
push constant 5
add
push this 1
push constant 5
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Ball.getLeft 0
push argument 0
pop pointer 0
push this 0
return
function Ball.getRight 0
push argument 0
pop pointer 0
push this 0
push constant 5
add
return
function Ball.setDestination 3
push argument 0
pop pointer 0
push argument 1
push this 0
sub
pop this 2
push argument 2
push this 1
sub
pop this 3
push this 2
call Math.abs 1
pop local 0
push this 3
call Math.abs 1
pop local 1
push local 0
push local 1
lt
pop this 7
push this 7
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push local 0
pop local 2
push local 1
pop local 0
push local 2
pop local 1
push this 1
push argument 2
lt
pop this 8
push this 0
push argument 1
lt
pop this 9
goto IF_END0
label IF_FALSE0
push this 0
push argument 1
lt
pop this 8
push this 1
push argument 2
lt
pop this 9
label IF_END0
push constant 2
push local 1
call Math.multiply 2
push local 0
sub
pop this 4
push constant 2
push local 1
call Math.multiply 2
pop this 5
push constant 2
push local 1
push local 0
sub
call Math.multiply 2
pop this 6
push constant 0
return
function Ball.move 0
push argument 0
pop pointer 0
push pointer 0
call Ball.hide 1
pop temp 0
push this 4
push constant 0
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 4
push this 5
add
pop this 4
goto IF_END0
label IF_FALSE0
push this 4
push this 6
add
pop this 4
push this 9
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 7
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push this 0
push constant 4
add
pop this 0
goto IF_END2
label IF_FALSE2
push this 1
push constant 4
add
pop this 1
label IF_END2
goto IF_END1
label IF_FALSE1
push this 7
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push this 0
push constant 4
sub
pop this 0
goto IF_END3
label IF_FALSE3
push this 1
push constant 4
sub
pop this 1
label IF_END3
label IF_END1
label IF_END0
push this 8
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push this 7
if-goto IF_TRUE5
goto IF_FALSE5
label IF_TRUE5
push this 1
push constant 4
add
pop this 1
goto IF_END5
label IF_FALSE5
push this 0
push constant 4
add
pop this 0
label IF_END5
goto IF_END4
label IF_FALSE4
push this 7
if-goto IF_TRUE6
goto IF_FALSE6
label IF_TRUE6
push this 1
push constant 4
sub
pop this 1
goto IF_END6
label IF_FALSE6
push this 0
push constant 4
sub
pop this 0
label IF_END6
label IF_END4
push this 0
push this 10
gt
not
if-goto IF_TRUE7
goto IF_FALSE7
label IF_TRUE7
push constant 1
pop this 14
push this 10
pop this 0
label IF_FALSE7
push this 0
push this 11
lt
not
if-goto IF_TRUE8
goto IF_FALSE8
label IF_TRUE8
push constant 2
pop this 14
push this 11
pop this 0
label IF_FALSE8
push this 1
push this 12
gt
not
if-goto IF_TRUE9
goto IF_FALSE9
label IF_TRUE9
push constant 3
pop this 14
push this 12
pop this 1
label IF_FALSE9
push this 1
push this 13
lt
not
if-goto IF_TRUE10
goto IF_FALSE10
label IF_TRUE10
push constant 4
pop this 14
push this 13
pop this 1
label IF_FALSE10
push pointer 0
call Ball.show 1
pop temp 0
push this 14
return
function Ball.bounce 5
push argument 0
pop pointer 0
push this 2
push constant 10
call Math.divide 2
pop local 2
push this 3
push constant 10
call Math.divide 2
pop local 3
push argument 1
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 10
pop local 4
goto IF_END0
label IF_FALSE0
push this 2
push constant 0
lt
not
push argument 1
push constant 1
eq
and
push this 2
push constant 0
lt
push argument 1
push constant 1
neg
eq
and
or
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 20
pop local 4
goto IF_END1
label IF_FALSE1
push constant 5
pop local 4
label IF_END1
label IF_END0
push this 14
push constant 1
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push constant 506
pop local 0
push local 3
push constant 50
neg
call Math.multiply 2
push local 2
call Math.divide 2
pop local 1
push this 1
push local 1
push local 4
call Math.multiply 2
add
pop local 1
goto IF_END2
label IF_FALSE2
push this 14
push constant 2
eq
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push constant 0
pop local 0
push local 3
push constant 50
call Math.multiply 2
push local 2
call Math.divide 2
pop local 1
push this 1
push local 1
push local 4
call Math.multiply 2
add
pop local 1
goto IF_END3
label IF_FALSE3
push this 14
push constant 3
eq
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push constant 250
pop local 1
push local 2
push constant 25
neg
call Math.multiply 2
push local 3
call Math.divide 2
pop local 0
push this 0
push local 0
push local 4
call Math.multiply 2
add
pop local 0
goto IF_END4
label IF_FALSE4
push constant 0
pop local 1
push local 2
push constant 25
call Math.multiply 2
push local 3
call Math.divide 2
pop local 0
push this 0
push local 0
push local 4
call Math.multiply 2
add
pop local 0
label IF_END4
label IF_END3
label IF_END2
push pointer 0
push local 0
push local 1
call Ball.setDestination 3
pop temp 0
push constant 0
return
");
	}

	[Fact]
	public void CompilePongGame()
	{
		var source = @"
			class PongGame {

				static PongGame instance; // the singleton, a Pong game instance     
				field Bat bat;            // the bat
				field Ball ball;          // the ball
				field int wall;           // the current wall that the ball is bouncing off of.
				field boolean exit;       // true when the game is over
				field int score;          // the current score.
				field int lastWall;       // the last wall that the ball bounced off of.

				// The current width of the bat
				field int batWidth;

				/** Constructs a new Pong game. */
				constructor PongGame new() {
					do Screen.clearScreen();
					let batWidth = 50;  // initial bat size
					let bat = Bat.new(230, 229, batWidth, 7);
					let ball = Ball.new(253, 222, 0, 511, 0, 229);
					do ball.setDestination(400,0);
					do Screen.drawRectangle(0, 238, 511, 240);
					do Output.moveCursor(22,0);
					do Output.printString(""Score: 0"");
				
					let exit = false;
					let score = 0;
					let wall = 0;
					let lastWall = 0;

					return this;
				}

				/** Deallocates the object's memory. */
				method void dispose() {
					do bat.dispose();
					do ball.dispose();
					do Memory.deAlloc(this);
					return;
				}

				/** Creates an instance of Pong game, and stores it. */
				function void newInstance() {
					let instance = PongGame.new();
					return;
				}
				
				/** Returns the single instance of this Pong game. */
				function PongGame getInstance() {
					return instance;
				}

				/** Starts the game, and andles inputs from the user that control
				 *  the bat's movement direction. */
				method void run() {
					var char key;

					while (~exit) {
						// waits for a key to be pressed.
						while ((key = 0) & (~exit)) {
							let key = Keyboard.keyPressed();
							do bat.move();
							do moveBall();
							do Sys.wait(50);
						}

						if (key = 130) { do bat.setDirection(1); }
						else {
							if (key = 132) { do bat.setDirection(2); }
							else {
								if (key = 140) { let exit = true; }
							}
						}

						// Waits for the key to be released.
						while ((~(key = 0)) & (~exit)) {
							let key = Keyboard.keyPressed();
							do bat.move();
							do moveBall();
							do Sys.wait(50);
						}
					}

					if (exit) {
						do Output.moveCursor(10,27);
						do Output.printString(""Game Over"");
					}
						
					return;
				}

				/**
				 * Handles ball movement, including bouncing.
				 * If the ball bounces off a wall, finds its new direction.
				 * If the ball bounces off the bat, increases the score by one
				 * and shrinks the bat's size, to make the game more challenging. 
				 */
				method void moveBall() {
					var int bouncingDirection, batLeft, batRight, ballLeft, ballRight;

					let wall = ball.move();

					if ((wall > 0) & (~(wall = lastWall))) {
						let lastWall = wall;
						let bouncingDirection = 0;
						let batLeft = bat.getLeft();
						let batRight = bat.getRight();
						let ballLeft = ball.getLeft();
						let ballRight = ball.getRight();
			  
						if (wall = 4) {
							let exit = (batLeft > ballRight) | (batRight < ballLeft);
							if (~exit) {
								if (ballRight < (batLeft + 10)) { let bouncingDirection = -1; }
								else {
									if (ballLeft > (batRight - 10)) { let bouncingDirection = 1; }
								}

								let batWidth = batWidth - 2;
								do bat.setWidth(batWidth);      
								let score = score + 1;
								do Output.moveCursor(22,7);
								do Output.printInt(score);
							}
						}
						do ball.bounce(bouncingDirection);
					}
					return;
				}
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function PongGame.new 0
push constant 7
call Memory.alloc 1
pop pointer 0
call Screen.clearScreen 0
pop temp 0
push constant 50
pop this 6
push constant 230
push constant 229
push this 6
push constant 7
call Bat.new 4
pop this 0
push constant 253
push constant 222
push constant 0
push constant 511
push constant 0
push constant 229
call Ball.new 6
pop this 1
push this 1
push constant 400
push constant 0
call Ball.setDestination 3
pop temp 0
push constant 0
push constant 238
push constant 511
push constant 240
call Screen.drawRectangle 4
pop temp 0
push constant 22
push constant 0
call Output.moveCursor 2
pop temp 0
push constant 8
call String.new 1
push constant 83
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 111
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 48
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 0
pop this 3
push constant 0
pop this 4
push constant 0
pop this 2
push constant 0
pop this 5
push pointer 0
return
function PongGame.dispose 0
push argument 0
pop pointer 0
push this 0
call Bat.dispose 1
pop temp 0
push this 1
call Ball.dispose 1
pop temp 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function PongGame.newInstance 0
call PongGame.new 0
pop static 0
push constant 0
return
function PongGame.getInstance 0
push static 0
return
function PongGame.run 1
push argument 0
pop pointer 0
label WHILE_EXP0
push this 3
not
not
if-goto WHILE_END0
label WHILE_EXP1
push local 0
push constant 0
eq
push this 3
not
and
not
if-goto WHILE_END1
call Keyboard.keyPressed 0
pop local 0
push this 0
call Bat.move 1
pop temp 0
push pointer 0
call PongGame.moveBall 1
pop temp 0
push constant 50
call Sys.wait 1
pop temp 0
goto WHILE_EXP1
label WHILE_END1
push local 0
push constant 130
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 0
push constant 1
call Bat.setDirection 2
pop temp 0
goto IF_END0
label IF_FALSE0
push local 0
push constant 132
eq
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 0
push constant 2
call Bat.setDirection 2
pop temp 0
goto IF_END1
label IF_FALSE1
push local 0
push constant 140
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push constant 0
not
pop this 3
label IF_FALSE2
label IF_END1
label IF_END0
label WHILE_EXP2
push local 0
push constant 0
eq
not
push this 3
not
and
not
if-goto WHILE_END2
call Keyboard.keyPressed 0
pop local 0
push this 0
call Bat.move 1
pop temp 0
push pointer 0
call PongGame.moveBall 1
pop temp 0
push constant 50
call Sys.wait 1
pop temp 0
goto WHILE_EXP2
label WHILE_END2
goto WHILE_EXP0
label WHILE_END0
push this 3
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push constant 10
push constant 27
call Output.moveCursor 2
pop temp 0
push constant 9
call String.new 1
push constant 71
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 109
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 79
call String.appendChar 2
push constant 118
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 114
call String.appendChar 2
call Output.printString 1
pop temp 0
label IF_FALSE3
push constant 0
return
function PongGame.moveBall 5
push argument 0
pop pointer 0
push this 1
call Ball.move 1
pop this 2
push this 2
push constant 0
gt
push this 2
push this 5
eq
not
and
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 2
pop this 5
push constant 0
pop local 0
push this 0
call Bat.getLeft 1
pop local 1
push this 0
call Bat.getRight 1
pop local 2
push this 1
call Ball.getLeft 1
pop local 3
push this 1
call Ball.getRight 1
pop local 4
push this 2
push constant 4
eq
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push local 1
push local 4
gt
push local 2
push local 3
lt
or
pop this 3
push this 3
not
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push local 4
push local 1
push constant 10
add
lt
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push constant 1
neg
pop local 0
goto IF_END3
label IF_FALSE3
push local 3
push local 2
push constant 10
sub
gt
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push constant 1
pop local 0
label IF_FALSE4
label IF_END3
push this 6
push constant 2
sub
pop this 6
push this 0
push this 6
call Bat.setWidth 2
pop temp 0
push this 4
push constant 1
add
pop this 4
push constant 22
push constant 7
call Output.moveCursor 2
pop temp 0
push this 4
call Output.printInt 1
pop temp 0
label IF_FALSE2
label IF_FALSE1
push this 1
push local 0
call Ball.bounce 2
pop temp 0
label IF_FALSE0
push constant 0
return
");
	}

	[Fact]
	public void CompilePongBat()
	{
		var source = @"
			class Bat {

				field int x, y;           // the bat's screen location
				field int width, height;  // the bat's width and height
				field int direction;      // direction of the bat's movement (1 = left, 2 = right)

				/** Constructs a new bat with the given location and width. */
				constructor Bat new(int Ax, int Ay, int Awidth, int Aheight) {
					let x = Ax;
					let y = Ay;
					let width = Awidth;
					let height = Aheight;
					let direction = 2;
					do show();
					return this;
				}

				/** Deallocates the object's memory. */
				method void dispose() {
					do Memory.deAlloc(this);
					return;
				}

				/** Shows the bat. */
				method void show() {
					do Screen.setColor(true);
					do draw();
					return;
				}

				/** Hides the bat. */
				method void hide() {
					do Screen.setColor(false);
					do draw();
					return;
				}

				/** Draws the bat. */
				method void draw() {
					do Screen.drawRectangle(x, y, x + width, y + height);
					return;
				}

				/** Sets the bat's direction (0=stop, 1=left, 2=right). */
				method void setDirection(int Adirection) {
					let direction = Adirection;
					return;
				}

				/** Returns the bat's left edge. */
				method int getLeft() {
					return x;
				}

				/** Returns the bat's right edge. */
				method int getRight() {
					return x + width;
				}

				/** Sets the bat's width. */
				method void setWidth(int Awidth) {
					do hide();
					let width = Awidth;
					do show();
					return;
				}

				/** Moves the bat one step in the bat's direction. */
				method void move() {
					if (direction = 1) {
						let x = x - 4;
						if (x < 0) { let x = 0; }
						do Screen.setColor(false);
						do Screen.drawRectangle((x + width) + 1, y, (x + width) + 4, y + height);
						do Screen.setColor(true);
						do Screen.drawRectangle(x, y, x + 3, y + height);
					}
					else {
						let x = x + 4;
						if ((x + width) > 511) { let x = 511 - width; }
						do Screen.setColor(false);
						do Screen.drawRectangle(x - 4, y, x - 1, y + height);
						do Screen.setColor(true);
						do Screen.drawRectangle((x + width) - 3, y, x + width, y + height);
					}
					return;
				}
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function Bat.new 0
push constant 5
call Memory.alloc 1
pop pointer 0
push argument 0
pop this 0
push argument 1
pop this 1
push argument 2
pop this 2
push argument 3
pop this 3
push constant 2
pop this 4
push pointer 0
call Bat.show 1
pop temp 0
push pointer 0
return
function Bat.dispose 0
push argument 0
pop pointer 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function Bat.show 0
push argument 0
pop pointer 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push pointer 0
call Bat.draw 1
pop temp 0
push constant 0
return
function Bat.hide 0
push argument 0
pop pointer 0
push constant 0
call Screen.setColor 1
pop temp 0
push pointer 0
call Bat.draw 1
pop temp 0
push constant 0
return
function Bat.draw 0
push argument 0
pop pointer 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push this 3
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Bat.setDirection 0
push argument 0
pop pointer 0
push argument 1
pop this 4
push constant 0
return
function Bat.getLeft 0
push argument 0
pop pointer 0
push this 0
return
function Bat.getRight 0
push argument 0
pop pointer 0
push this 0
push this 2
add
return
function Bat.setWidth 0
push argument 0
pop pointer 0
push pointer 0
call Bat.hide 1
pop temp 0
push argument 1
pop this 2
push pointer 0
call Bat.show 1
pop temp 0
push constant 0
return
function Bat.move 0
push argument 0
pop pointer 0
push this 4
push constant 1
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 0
push constant 4
sub
pop this 0
push this 0
push constant 0
lt
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 0
pop this 0
label IF_FALSE1
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 2
add
push constant 1
add
push this 1
push this 0
push this 2
add
push constant 4
add
push this 1
push this 3
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push constant 3
add
push this 1
push this 3
add
call Screen.drawRectangle 4
pop temp 0
goto IF_END0
label IF_FALSE0
push this 0
push constant 4
add
pop this 0
push this 0
push this 2
add
push constant 511
gt
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push constant 511
push this 2
sub
pop this 0
label IF_FALSE2
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push constant 4
sub
push this 1
push this 0
push constant 1
sub
push this 1
push this 3
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 2
add
push constant 3
sub
push this 1
push this 0
push this 2
add
push this 1
push this 3
add
call Screen.drawRectangle 4
pop temp 0
label IF_END0
push constant 0
return
");
	}

	[Fact]
	public void CompileMath()
	{
		var source = @"
			// This file is part of www.nand2tetris.org
			// and the book ""The Elements of Computing Systems""
			// by Nisan and Schocken, MIT Press.
			// File name: projects/12/Math.jack

			/**
			 * A library of commonly used mathematical functions.
			 * Note: Jack compilers implement multiplication and division using OS method calls.
			 */
			class Math {
				static Array twoToThe;
				/** Initializes the library. */
				function void init() {
					let twoToThe = Array.new(16);
					let twoToThe[0] = 1;
					let twoToThe[1] = 2;
					let twoToThe[2] = 4;
					let twoToThe[3] = 8;
					let twoToThe[4] = 16;
					let twoToThe[5] = 32;
					let twoToThe[6] = 64;
					let twoToThe[7] = 128;
					let twoToThe[8] = 256;
					let twoToThe[9] = 512;
					let twoToThe[10] = 1024;
					let twoToThe[11] = 2048;
					let twoToThe[12] = 4096;
					let twoToThe[13] = 8192;
					let twoToThe[14] = 16384;
					//let twoToThe[15] = 32768;

					return;
				}

				/** Returns the absolute value of x. */
				function int abs(int x) {
					if (x < 0) {
						return -x;
					}

					return x;
				}

				/** Returns the product of x and y. 
				 *  When a Jack compiler detects the multiplication operator '*' in the 
				 *  program's code, it handles it by invoking this method. In other words,
				 *  the Jack expressions x*y and multiply(x,y) return the same value.
				 */
				function int multiply(int x, int y) {
					/*
						multiply (x, y):
							sum = 0
							shiftedX = x
							for i = 0 ... w - 1 do
								if ((i'th bit of y) == 1)
									sum = sum + shiftedX
								shiftedX = shiftedX * 2
							return sum
					*/

					var int i, sum, shiftedX, w;
					let sum = 0;
					let shiftedX = x;
					let i = 0;
					let w = 16;

					while (i < w) {
						if (Math.bit(y, i)) {
							let sum = sum +  shiftedX;
						}
						let shiftedX = shiftedX + shiftedX;
						
						let i = i + 1;
					}

					return sum;
				}

				/** Returns the integer part of x/y.
				 *  When a Jack compiler detects the multiplication operator '/' in the 
				 *  program's code, it handles it by invoking this method. In other words,
				 *  the Jack expressions x/y and divide(x,y) return the same value.
				 */
				function int divide(int x, int y) {
					/*
						if (x > y) return 0
						q = divide(x, 2 * y)
						if ((x - 2 * q * y) < y)
							return 2 * q
						else
							return 2 * q + 1
					*/

					var int q, absX, absY, result;
					let absX = Math.abs(x);
					let absY = Math.abs(y);

					if (absX > y) {
						return 0;
					}

					let q = Math.divide(absX, 2 * absY);
					if (((absX - 2) * (q * absY)) < absY) {
						return 2 * q;
					}

					let result =  2 * q + 1;

					if (result < 0) {
						return 0;
					}

					if ((((x < 0) & (y > 0)) | ((x > 0) & (y < 0))) & ~(result = 0)) {
						let result = -result;
					}
					return result;
				}

				/** Returns the integer part of the square root of x. */
				function int sqrt(int x) {
					/*
						sqrt (x):
							y = 0
							for j = x/2 -1 ...0 do
								if (x + 2^j)^2 <= x then y = y + 2^j

						return y
					*/

					var int test1, j, y, i, whileTest;
					let j = (x / 2) - 1;
					let y = 0;

					let test1 = Math.power(Math.power((y + 2), j), 2);

					if ((test1 = 0) | test1 < 0) {
						return 0;
					}

					while (i > 0) {
						let whileTest = Math.power(Math.power(x+2, j), 2);
						if ((whileTest < x) | (whileTest = x)) {
							let y = y + Math.power(2, j);
						}

						let i = i -1;
					}
					return y;
				}

				/** Returns the greater number. */
				function int max(int a, int b) {
					if (a > b) {
						return a;
					}

					return b;
				}

				/** Returns the smaller number. */
				function int min(int a, int b) {
					if (a < b) {
						return a;
					}

					return b;
				}

				// Returns true if the i-th bit of x is 1, false otherwise
				function boolean bit(int x, int i){
					var int bit;
					let bit = x & twoToThe[i];

					return ~(bit = 0);
				}

				// return x^y
				function int power(int x, int y) {
					var int i, result, test1, test2;
					let i = 0;
					let result = x;
					
					if (y = 0) {
						return 1;
					}

					while (i < (y - 1)) {
						let result = result * x;
						let i = i + 1;
					}

					return result;
				}
			}

			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
			@"function Math.init 0
push constant 16
call Array.new 1
pop static 0
push constant 0
push static 0
add
push constant 1
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 1
push static 0
add
push constant 2
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 2
push static 0
add
push constant 4
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push static 0
add
push constant 8
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 4
push static 0
add
push constant 16
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 5
push static 0
add
push constant 32
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 6
push static 0
add
push constant 64
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 7
push static 0
add
push constant 128
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 8
push static 0
add
push constant 256
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 9
push static 0
add
push constant 512
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 10
push static 0
add
push constant 1024
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 11
push static 0
add
push constant 2048
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 12
push static 0
add
push constant 4096
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 13
push static 0
add
push constant 8192
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 14
push static 0
add
push constant 16384
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 0
return
function Math.abs 0
push argument 0
push constant 0
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push argument 0
neg
return
label IF_FALSE0
push argument 0
return
function Math.multiply 4
push constant 0
pop local 1
push argument 0
pop local 2
push constant 0
pop local 0
push constant 16
pop local 3
label WHILE_EXP0
push local 0
push local 3
lt
not
if-goto WHILE_END0
push argument 1
push local 0
call Math.bit 2
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push local 1
push local 2
add
pop local 1
label IF_FALSE0
push local 2
push local 2
add
pop local 2
push local 0
push constant 1
add
pop local 0
goto WHILE_EXP0
label WHILE_END0
push local 1
return
function Math.divide 4
push argument 0
call Math.abs 1
pop local 1
push argument 1
call Math.abs 1
pop local 2
push local 1
push argument 1
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
return
label IF_FALSE0
push local 1
push constant 2
push local 2
call Math.multiply 2
call Math.divide 2
pop local 0
push local 1
push constant 2
sub
push local 0
push local 2
call Math.multiply 2
call Math.multiply 2
push local 2
lt
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 2
push local 0
call Math.multiply 2
return
label IF_FALSE1
push constant 2
push local 0
call Math.multiply 2
push constant 1
add
pop local 3
push local 3
push constant 0
lt
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push constant 0
return
label IF_FALSE2
push argument 0
push constant 0
lt
push argument 1
push constant 0
gt
and
push argument 0
push constant 0
gt
push argument 1
push constant 0
lt
and
or
push local 3
push constant 0
eq
not
and
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push local 3
neg
pop local 3
label IF_FALSE3
push local 3
return
function Math.sqrt 5
push argument 0
push constant 2
call Math.divide 2
push constant 1
sub
pop local 1
push constant 0
pop local 2
push local 2
push constant 2
add
push local 1
call Math.power 2
push constant 2
call Math.power 2
pop local 0
push local 0
push constant 0
eq
push local 0
or
push constant 0
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
return
label IF_FALSE0
label WHILE_EXP0
push local 3
push constant 0
gt
not
if-goto WHILE_END0
push argument 0
push constant 2
add
push local 1
call Math.power 2
push constant 2
call Math.power 2
pop local 4
push local 4
push argument 0
lt
push local 4
push argument 0
eq
or
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push local 2
push constant 2
push local 1
call Math.power 2
add
pop local 2
label IF_FALSE1
push local 3
push constant 1
sub
pop local 3
goto WHILE_EXP0
label WHILE_END0
push local 2
return
function Math.max 0
push argument 0
push argument 1
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push argument 0
return
label IF_FALSE0
push argument 1
return
function Math.min 0
push argument 0
push argument 1
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push argument 0
return
label IF_FALSE0
push argument 1
return
function Math.bit 1
push argument 0
push argument 1
push static 0
add
pop pointer 1
push that 0
and
pop local 0
push local 0
push constant 0
eq
not
return
function Math.power 4
push constant 0
pop local 0
push argument 0
pop local 1
push argument 1
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 1
return
label IF_FALSE0
label WHILE_EXP0
push local 0
push argument 1
push constant 1
sub
lt
not
if-goto WHILE_END0
push local 1
push argument 0
call Math.multiply 2
pop local 1
push local 0
push constant 1
add
pop local 0
goto WHILE_EXP0
label WHILE_END0
push local 1
return
");
	}
}