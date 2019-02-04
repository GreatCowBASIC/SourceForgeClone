BEGIN{
    if( NoHeaderMessage == 0 ) {
      print "Great Cow BASIC preprocessor (v0.98.04)"
      #24.09.2016   'revised to correct detection error
      #29.05.2017   'Remove ExamineElseError. Removed this check at v0.98.01 as we now support if-then-else-if-end_if
      #29.01.2019   'Remove ExamineElse. Reverted this check at v0.98.05 as we now newed to a smart ELSE IF check
      #30.01.2019   'Remove errors in processing rules.

    }

      B["("]=")";
      #B["["]="]";
      #B["{"]="}";
      exitcode = 1;
    vfsp = FILENAME;



    }

    #Main
    {
      fsp = basename( FILENAME );
      #print fsp;
      #print $0
      commentline = 0;
      newline = toupper(trim($0));
      #print newline;
      #commentline = 0;
      if ( substr( newline, 1 , 1 ) == "'" )
        commentline = 1
      if ( substr( newline, 1 , 1 ) == ";" )
        commentline = 1
      if ( substr( newline, 1 , 3 ) == "REM" )
        commentline = 1

      #print commentline;
      #print $0
      if ( commentline != 1 ) {
        if ( length(newline) > 0 ) {
            #ExamineElseError();  #Removed this check at v0.98.01 RC01 as we now support if-then-else-if-end_if
            ExamineBraces();
            ExamineIfs();
            ExamineElse();
            ExamineCasting();
            ExamineLoops()
            #ExamineMethods uses global variable to set count. If count != then we have an issue.
            ExamineMethods();
        }
      }



    }


END {

    if ( subcounter != 0 || funccounter != 0 ) {
        #printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast);
        printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast) >> "Errors.txt";
        exitcode = 1;
    }


    if( NoHeaderMessage == 0 ) {
      print "\tStandard include files OK"
    }

      exit exitcode;
    }



    function ltrim(s) { sub(/^[ \t\r\n]+/, "", s); return s }
    function rtrim(s) { sub(/[ \t\r\n]+$/, "", s); return s }
    function trim(s)  { return rtrim(ltrim(s)); }

function basename(file, a, n) {
    n = split(file, a, "\\")
    return a[n]
  }

#check for multiple if statements
function ExamineIfs() {

  if ( index(toupper(newline),"IF") == 1 ) {
    #get first char
    commentline = 0;
    newline = toupper(trim(newline));
    if ( length( newline ) > 0 ) {
        if ( substr( newline, 1 , 1 ) == "'" )
          commentline = 1
        if ( substr( newline, 1 , 1 ) == ";" )
          commentline = 1
        if ( substr( newline, 1 , 3 ) == "REM" )
          commentline = 1

        #find silly syntax
        sillysyntax  = index(toupper(newline),":ELSE")
        #find comment field
        singlequote  = index(toupper(newline),"'")
        semicolon  = index(toupper(newline),";")
        remstatement  = index(toupper(newline),"REM")
        if ( sillysyntax !=0 ) {
            #not in comments
            if (  sillysyntax < singlequote ||  sillysyntax < semicolon || sillysyntax < remstatement ) {
              commentline = 2
            }
            #no comments on this line... must be incorrect syntax
            if ( singlequote + semicolon + remstatement == 0 ) {
                commentline = 2
            }
        }
        #find addition IFs
        for ( nn=2 ; nn <= NF ; nn++ ){
              #print toupper( $nn )
              if ( substr( $nn, 1 , 1 ) == "'" || substr( $nn, 1 , 1 ) == ";" || substr( $nn, 1 , 3 ) == "REM" ) {
                  #its a comment, exit
                  nn = NF+1
                  }
              else {
                  if ( toupper( $nn ) == "IF" ) {
                      commentline++;
                  }
              }
            }

        if ( commentline ==1 ) {
              printf("%s (%s): Syntax Error:  Mutliple IFs not permitted. .\n", fsp, NR) >> "Errors.txt";
              exitcode = 1;
        }

        if ( commentline ==2 ) {
              printf("%s (%s): Syntax Error:  ELSE statement not permitted. .\n", fsp, NR) >> "Errors.txt";
              exitcode = 1;
        }
    }
  }
}



function ExamineElse() {

  if ( index(toupper(newline),"ELSE") == 1 ) {
    newline = trim(substr( newline, 5 ));
    #get first char
    commentline = 0;
    newline = toupper(trim(newline));
    if ( length( newline ) > 0 ) {
        if ( substr( newline, 1 , 1 ) == "'" )
          commentline = 1
        if ( substr( newline, 1 , 1 ) == ";" )
          commentline = 1
        if ( substr( newline, 1 , 3 ) == "REM" )
          commentline = 1
        if ( ( toupper($2) == "IF" ) && ( NF == 2 ) )
          commentline = 2
        if ( ( toupper($2) == "IF" ) && ( NF > 2 ) && ( toupper($NF) ==  "THEN" )  )
          commentline = 1
        if ( ( toupper($2) == "IF" ) && ( NF == 3 ) && ( toupper($NF) ==  "THEN" )  )
          commentline = 3
        if ( ( toupper($1) == "ELSE" ) && ( NF == 1 )  )
          commentline = 1

        if ( commentline == 1 ) {
          #a comment
        }
        else {
        if ( commentline == 2 ) {
            printf("%s (%s): Error:  ELSE IF has incorrect syntax. <Condition> THEN required .\n", fsp, NR) >> "Errors.txt";
            exitcode = 1;
            }
        else {

            if ( commentline == 3 ) {
              printf("%s (%s): Error:  ELSE IF has no condition. Condition required.\n", fsp, NR) >> "Errors.txt";
              exitcode = 1;
            }
            else {
                printf("%s (%s): Error:  If-Else line has continous code - not permitted. Place code on next line\n", fsp, NR) >> "Errors.txt";
                exitcode = 1;
              }
            }
        }
    }
  }
}


function ExamineBraces() {
  #printf ( "%s\n",newline);
  #Process line
  lbraces = gsub(/\(/, "(", newline );
  rbraces = gsub(/\)/, ")", newline );

  if ( ( lbraces + rbraces ) != 0 ) {
    #printf ( "%s\n",newline);
    lbraces = 0;
    rbraces = 0;
    handlingstring = 0;

    # process a line
    for ( nn=1 ; nn <= length(newline) ; nn++ ){
      nextchar = substr( newline, nn, 1 );
      nextchars = toupper(substr( newline, nn, 2 ));
      #print nextchar
      #print nextchars

      if ( nextchar == "\"")  {

         #consume until next quote
         do {
              nn++;
              nextchar = substr( newline, nn, 1 );
              #print nextchar;
               if ( nextchar == "\"")  {
                break
               }

         } while ( nn <= length(newline) )
      }

      else if ( nextchar == ";")  {

          nn = length ( newline );
          break;
      }
      else if  ( nextchar == "'")  {
        #process '
        nn = length(newline) + 1
      }
      else if ( nextchar == "(" ) {
         lbraces ++;
      }
      else if ( nextchar == ")" ) {
         rbraces ++;
      }

    }

    #count the remaining braces
    #lbraces = gsub(/\(/, "(", newline );
    #rbraces = gsub(/\)/, ")", newline );

    if ( ( lbraces != rbraces )  ) {
        printf("%s (%s): Error: 2. Unbalanced Braces\n", fsp, NR) >> "Errors.txt";
        exitcode = 2;
    }
  }
}


function ExamineCasting() {
  #printf ( "%s\n",newline);
  #Process line
  lbraces = gsub(/\[/, "(", newline );
  rbraces = gsub(/\]/, ")", newline );



  if ( ( lbraces + rbraces ) != 0 ) {
    #printf ( "%s\n",newline);
    lbraces = 0;
    rbraces = 0;
    handlingstring = 0;

    # process a line
    for ( nn=1 ; nn <= length(newline) ; nn++ ){
      nextchar = substr( newline, nn, 1 );
      #print nextchar
      if ( nextchar == "\"")  {

         #consume until next quote
         do {
              nn++;
              nextchar = substr( newline, nn, 1 );
              #print nextchar;
               if ( nextchar == "\"")  {
                break
               }

         } while ( nn <= length(newline) )
      }

      else if ( nextchar == ";")  {

          nn = length ( newline );
          break;
      }
      else if  ( nextchar == "'")  {
      #pr0cess b'00000000'
        #nn = length(newline) + 1

        #consume until next single quote
         do {
              nn++;
              nextchar = substr( newline, nn, 1 );
              #print nextchar;
               if ( nextchar == "'")  {
                break
               }

         } while ( nn <= length(newline) )

      }
      else if ( nextchar == "(" ) {
         lbraces ++;
      }
      else if ( nextchar == ")" ) {
         rbraces ++;
      }

    }

    #count the remaining braces
    #lbraces = gsub(/\(/, "(", newline );
    #rbraces = gsub(/\)/, ")", newline );

    if ( ( lbraces != rbraces )  ) {
        printf("%s (%s): Error: 3. Unbalanced Braces\n", fsp, NR) >> "Errors.txt";
        exitcode = 2;
    }
  }
}



function ExamineLoops() {
  #printf ( "%s\n",newline);
  #Process line
  lbraces = gsub(/\{/, "(", newline );
  rbraces = gsub(/\}/, ")", newline );



  if ( ( lbraces + rbraces ) != 0 ) {
    #printf ( "%s\n",newline);
    lbraces = 0;
    rbraces = 0;
    handlingstring = 0;

    # process a line
    for ( nn=1 ; nn <= length(newline) ; nn++ ){
      nextchar = substr( newline, nn, 1 );
      #print nextchar
      if ( nextchar == "\"")  {

         #consume until next quote
         do {
              nn++;
              nextchar = substr( newline, nn, 1 );
              #print nextchar;
               if ( nextchar == "\"")  {
                break
               }

         } while ( nn <= length(newline) )
      }

      else if ( nextchar == ";")  {

          nn = length ( newline );
          break;
      }
      else if  ( nextchar == "'")  {
      #pr0cess b'00000000'
        #nn = length(newline) + 1

        #consume until next single quote
         do {
              nn++;
              nextchar = substr( newline, nn, 1 );
              #print nextchar;
               if ( nextchar == "'")  {
                break
               }

         } while ( nn <= length(newline) )

      }
      else if ( nextchar == "(" ) {
         lbraces ++;
      }
      else if ( nextchar == ")" ) {
         rbraces ++;
      }

    }

    #count the remaining braces
    #lbraces = gsub(/\(/, "(", newline );
    #rbraces = gsub(/\)/, ")", newline );

    if ( ( lbraces != rbraces )  ) {
        printf("%s (%s): Error: 1. Unbalanced Braces\n", fsp, NR) >> "Errors.txt";
        exitcode = 2;
    }
  }
}


function ExamineElseError() {

  if ( index(toupper(newline),"ELSE") == 1 ) {
    newline = trim(substr( newline, 5 ));
    #get first char
    commentline = 0;
    newline = toupper(trim(newline));
    if ( length( newline ) > 0 ) {
        if ( substr( newline, 1 , 1 ) == "'" )
          commentline = 1
        if ( substr( newline, 1 , 1 ) == ";" )
          commentline = 1
        if ( substr( newline, 1 , 3 ) == "REM" )
          commentline = 1

        if ( commentline == 1 ) {
          #a comment
        }
        else {
          #code
            #printf("%s (%s): Error: If-Else line has continous code - not permitted. Place code on next line\n", fsp, NR);
            printf("%s (%s): Error:  If-Else line has continous code - not permitted. Place code on next line\n", fsp, NR) >> "Errors.txt";
            exitcode = 2;
        }
    }
  }
}

function ExamineMethods() {

        newline = toupper(newline);

        if ( index( newline , "SUB ") == 1  ){
          subcounter++

          if ( subcounter != 1 ) {
            #printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast);
            printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast) >> "Errors.txt";
            sublast = NR
            exitcode = 2;
            subcounter = 0
            funccounter = 0

          }
          else {
            sublast = NR
          }
        }
        if ( index( newline , "END SUB") == 1  ){
            subcounter--
            if ( subcounter < 0 ) {
              #printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, NR);
              printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, NR) >> "Errors.txt";
              sublast = NR
              exitcode = 2;
              subcounter = 0
              funccounter = 0

            }
          }

        if ( index( newline , "FUNCTION ") == 1  ){
          funccounter++

          if ( funccounter != 1 ) {
            #printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast);
            printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast) >> "Errors.txt";
            sublast = NR
            exitcode = 2;
            funccounter = 0
            subcounter = 0
          }
          else {
            sublast = NR
          }
        }
        if ( index( newline , "END FUNCTION") == 1  ){
            funccounter--
            if ( funccounter < 0 ) {
              #printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast);
              printf("%s (%s): Warning: Missing Function/Sub definition\n", fsp, sublast) >> "Errors.txt";
              sublast = NR
              exitcode = 2;
              subcounter = 0
              funccounter = 0

            }

          }


}
