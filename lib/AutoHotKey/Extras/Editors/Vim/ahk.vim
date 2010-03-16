" Vim syntax file

" Language:	Autohotkey from www.autohotkey.com
" Maintainer:   savage - kallen19918 AT earthlink DOT net

"Usage:
"1) Copy this file into your $VIM/syntax directory.
"2) Add this line to filetype.vim:
"au BufNewFile,BufRead *.ahk setf ahk 



" For version 5.x: Clear all syntax items
" For version 6.x: Quit when a syntax file was already loaded
if version < 600
  syntax clear
elseif exists("b:current_syntax")
  finish
endif 

sy case ignore

sy keyword ahkKeyword ahk_id ahk_pid ahk_class ahk_group ahk_parent true false

sy match ahkFunction "^\s*\w\{1,},"
sy match ahkFunction "\w\{1,}," contained
sy match ahkFunction "^\s*\w\{1,}\s*$" contains=ahkStatement
sy match ahkFunction "\w\{1,}\s*$" contained

sy match ahkNewFunction "\s*\w\{1,}(.*)"
sy match ahkNewFunctionParams "(\@<=.*)\@=" containedin=ahkNewFunction

sy match ahkEscape "`." containedin=ahkFunction,ahkLabel,ahkVariable,ahkNewFunctionParams

sy match ahkVariable "%.*%" containedin=ahkNewFunctionParams
sy match ahkVariable "%.*%"



sy match ahkKey "[!#^+]\{1,4}`\=.\n" contains=ahkEscape
sy match ahkKey "[!#^+]\{0,4}{.\{-}}"


sy match ahkDirective "^#[a-zA-Z]\{2,\}"

sy match ahkLabel "^\w\+:\s*$"
sy match ahkLabel "^[^,]\+:\{2\}\(\w\+,\)\="  contains=ahkFunction
sy match ahkLabel "^[^,]\+:\{2\}\w\+\s*$" contains=ahkFunction
sy match ahkLabel "^:.\+:.*::"
sy keyword ahkLabel return containedin=ahkFunction

sy match ahkStatement "^\s*if\w*\(,\)\="
sy keyword ahkStatement If Else Loop Loop, exitapp containedin=ahkFunction

sy match ahkComment "`\@<!;.*" contains=NONE
sy match ahkComment "\/\*\_.\{-}\*\/" contains=NONE


hi def link ahkKeyword Special
hi def link ahkEscape Special
hi def link ahkComment Comment
hi def link ahkStatement Conditional
hi def link ahkFunction Type
hi def link ahkDirective Include
hi def link ahkLabel Label
hi def link ahkKey Special
hi def link ahkVariable Constant
hi def link ahkNewFunction Type

sy sync fromstart
let b:current_syntax = "ahk" 
