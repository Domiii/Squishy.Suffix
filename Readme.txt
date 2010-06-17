Suffix Tree implementation by Domi

Requirements:
	.NET Framework 4.0
	Visual Studio 2010 (or other IDE/make tool that supports VS 2010 project files)

SuffixTree references:
	http://www.csie.ntu.edu.tw/~hil/algo2010spring/2010spring-algo14.pptx
	http://www.csie.ntu.edu.tw/~hil/algo2010spring/2010spring-algo15.pptx
	http://www.cs.uku.fi/~kilpelai/BSA05/lectures/print07.pdf

Notes:
	1. The tree-building algorithms are in the SuffixTreeBuilder class: http://github.com/Domiii/Squishy.Suffix/blob/master/Squishy.Suffix/SuffixTreeBuilder.cs
	2. Every SuffixNode also represents the Edge towards its Parent (since it is a strong 1:1 relation)
	3. The code has a lot of documentation - Feel free to ask me, if anything is unclear

Sample Output:
	Sample tree 1: http://pastebin.com/JEw8Ym5a
	Sample tree 2: http://pastebin.com/NfPHwEh7
	Performance: http://pastebin.com/mY5ZnvGs