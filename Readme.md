Orcomp - Factor Oracle Compression / Compror Algorithm Implementation
------------------------------------------------------------------------

This repository contains a .Net port (from Java) of an Compror algorithm implementation. 

Compror makes use of factor oracle automata to implement an efficient lossless compression algorithm.

The compror algorithm was originally designed by Arnaud Lefebvre and Thierry Lecroq at Université de Rouen.
Their original paper is located at http://www-igm.univ-mlv.fr/~lecroq/articles/ipl4.pdf

This was originally done as a part of my BSc Comp Sci final year project back in 2004. 

------------------------------------------------------------------------

General notes on the code:
The original Java implementation (of which this is a fairly straight forward port, just here and there a couple of things were changed
to improve readability) was written 12 years ago - it's amazing to look at your own code that's that old. 

The code base is in need of some good refactoring, currently the main compression algorithm is located in the Orcalc class, while the 
decompression algorithm is located in the OracleDecoder - these implementations should be refactored to make their intent clearer. 
