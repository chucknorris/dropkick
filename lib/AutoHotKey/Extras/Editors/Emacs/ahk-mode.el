;;; ahk-mode.el --- major mode for editing AutoHotKey scripts for X/GNU Emacs

;; Copyright (C) 2005 Robert Widhopf-Fenk

;; Author:   Robert Widhopf-Fenk
;; Keywords: AutoHotKey, major mode
;; X-URL:    http://www.robf.de/Hacking/elisp
;; arch-tag: 1ae180cb-002e-4656-bd9e-a209acd4a3d4
;; Version:  $Id: ahk-mode--main--1.0--patch-4$

;; This code is free software; you can redistribute it and/or modify
;; it under the terms of the GNU General Public License as published by
;; the Free Software Foundation; either version 2, or (at your option)
;; any later version.
;;
;; This program is distributed in the hope that it will be useful,
;; but WITHOUT ANY WARRANTY; without even the implied warranty of
;; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;; GNU General Public License for more details.
;;

;;; Commentary:
;;
;; AutoHotKey: Automation, Hotkeys and Scripting for Windows at
;; http://www.autohotkey.com/ is a cool tool to make daily life
;; with Windows easier or even fun!
;;
;; This is a X/GNU Emacs mode for editing AutoHotKey scripts.
;;
;; Place this file somewhere in your load-path, byte-compile it and add the
;; following line to your ~/.xemacs/init.el resp. ~/.emacs:
;;
;; (setq ahk-syntax-directory "PATHTO/AutoHotkey/Extras/Editors/Syntax/")
;; (add-to-list 'auto-mode-alist '("\\.ahk$" . ahk-mode))
;; (autoload 'ahk-mode "ahk-mode")
;; 
;; The first time ahk-mode.el is started it will ask you for the path to the
;; Syntax directory if not set already.  You will find it as a subdirectory
;; of your AHK installation.
;;
;; For example if you installed AHK at C:\Programms\AutoHotKey it will be
;; C:/Programms/AutoHotKey/Extras/Editors/Syntax or a corresponding cygwin
;; path!
;;
;; When opening a script file you will get:
;; - syntax highlighting
;; - indention, completion and command help (bound to "TAB")
;; - insertion of command templates (bound to "C-c C-i") 
;; - electric braces (typing "{" will also insert "}" and place point in
;;   between) 
;; - lookup the docs on a command via w3 (place point on command and type
;;   "C-c C-h")
;;
;; Please send bug-reports or feature suggestions to hackATrobfDOTde.

;;; Bugs:
;;
;; - completions is not really context aware
;; - multi-line comments are not fontified correctly while editing,
;;   but only when fontifying the whole buffer.  If you know how to
;;   fix this, please let me know!

;;; History:
;;
;; The CHANGELOG is stored in my arch repository.
;;
;; If you wonder what arch is, take a look at http://wiki.gnuarch.org/ !

(eval-when-compile
  (require 'font-lock)
  (if (locate-library "w3") (require 'w3))
  (require 'cl))

;;; Code:
(defgroup ahk-mode nil
  "A mode for AutoHotKey"
  :group 'languages
  :prefix "ahk-")

(defcustom ahk-mode-hook '(ahk-mode-hook-activate-filling)
  "Hook functions run by `ahk-mode'."
  :type 'hook
  :group 'ahk-mode)

(defcustom ahk-indetion 2
  "The indetion level."
  :type 'integer
  :group 'ahk-mode)

(defcustom ahk-syntax-directory nil
  "The indetion level."
  :type 'directory
  :group 'ahk-mode)

;;;###autoload
(add-to-list 'auto-mode-alist
             '("\\.ahk$"  . ahk-mode))

(defvar ahk-mode-syntax-table
  (let ((table (make-syntax-table)))
    ;; these are also allowed in variable names
    (modify-syntax-entry ?#  "w" table)
    (modify-syntax-entry ?_  "w" table)
    (modify-syntax-entry ?@  "w" table)
    (modify-syntax-entry ?$  "w" table)
    (modify-syntax-entry ??  "w" table)
    (modify-syntax-entry ?[  "w" table)
    (modify-syntax-entry ?]  "w" table)
    ;; some additional characters used in paths and switches  
    (modify-syntax-entry ?\\  "w" table)
;    (modify-syntax-entry ?/  "w" table)
    (modify-syntax-entry ?-  "w" table)
    (modify-syntax-entry ?:  "w" table)
    (modify-syntax-entry ?.  "w" table)
    ;; for multiline comments (taken from cc-mode)
    (modify-syntax-entry ?/  ". 14" table)
    (modify-syntax-entry ?*  ". 23"   table)
    ;; Give CR the same syntax as newline, for selective-display
    (modify-syntax-entry ?\^m "> b" table)
    (modify-syntax-entry ?\n "> b"  table)
    table)
  "Syntax table used in `ahk-mode' buffers.")

(defvar ahk-mode-abbrev-table
  (let ((a (make-abbrev-table)))
    a)
  "Abbreviation table used in `ahk-mode' buffers.")

(defvar ahk-mode-map
  (let ((map (make-sparse-keymap)))
    (define-key map "\C-c\C-h" 'ahk-www-help-at-point)
    (define-key map "\C-c\C-c" 'ahk-comment-region)
    (define-key map "\C-c\C-i" 'ahk-insert-command-template)
    (define-key map "\t" 'ahk-indent-line-and-complete)
    (define-key map "{" 'ahk-electric-brace)
    (define-key map "}" 'ahk-electric-brace)
    (define-key map "\r" 'ahk-electric-return)
    map)
  "Keymap used in `ahk-mode' buffers.")

(defvar ahk-Commands-list nil
  "A list of ahk commands and parameters.
Will be initialized by `ahk-init'")

(defvar ahk-Keys-list nil
  "A list of ahk key names.
Will be initialized by `ahk-init'")

(defvar ahk-Keywords-list nil
  "A list of ahks keywords.
Will be initialized by `ahk-init'")

(defvar ahk-Variables-list nil
  "A list of ahks variables.
Will be initialized by `ahk-init'")

(defvar ahk-mode-font-lock-keywords nil
  "Syntax highlighting for `ahk-mode'.
Will be initialized by `ahk-init'")

(defvar ahk-completion-list nil
  "A list of all symbols available for completion
Will be initialized by `ahk-init'")

(easy-menu-define ahk-menu ahk-mode-map "AHK Mode Commands"
		  '("AHK"
                    ["Insert Command Template" ahk-insert-command-template]
                    ["Lookup webdocs on command" ahk-www-help-at-point]
                    ))

(defun ahk-init ()
  "Initialize ahk-mode variables.
An AHK installation provides a subdirectory \"Extras/Editors/Syntax\"
containing a list of keywords, variables, commands and keys.

This directory must be specified in the variable `ahk-syntax-directory'."
  (interactive)

  (message "Initializing ahk-mode variables ...")
  (when (null ahk-syntax-directory)
    (customize-save-variable
     'ahk-syntax-directory
     (read-file-name "Please give the AHK-Syntax directory: "))
    (custom-save-all))

  (save-excursion
    (set-buffer (get-buffer-create " *ahk-mode-temp*"))
  
    ;; read commands
    (erase-buffer)
    (insert-file-contents (expand-file-name "Commands.txt"
                                            ahk-syntax-directory))
    (setq ahk-Commands-list nil)
    (goto-char 0)
    (while (not (eobp))
      (if (not (looking-at "\\([^;\r\n][^(\t\r\n, ]+\\)\\([^\r\n]*\\)"))
          nil;; (error "Unknown file syntax")
        (setq ahk-Commands-list (cons (list
                                       (match-string 1)
                                       (match-string 2))
                                      ahk-Commands-list)))
      (forward-line 1))
    
    ;; read keys
    (erase-buffer)
    (insert-file-contents (expand-file-name "Keys.txt"
                                            ahk-syntax-directory))
    (setq ahk-Keys-list nil)
    (goto-char 0)
    (while (not (eobp))
      (if (not (looking-at "\\([^;\r\n][^\t\r\n ]+\\)"))
          nil;; (error "Unknown file syntax of Keys.txt")
        (setq ahk-Keys-list (cons (match-string 1) ahk-Keys-list)))
      (forward-line 1))
    
    ;; read keywords
    (erase-buffer)
    (insert-file-contents (expand-file-name "Keywords.txt"
                                            ahk-syntax-directory))
    (setq ahk-Keywords-list nil)
    (goto-char 0)
    (while (not (eobp))
      (if (not (looking-at "\\([^;\r\n][^\t\r\n ]+\\)"))
          nil;; (error "Unknown file syntax of Keywords.txt")
        (setq ahk-Keywords-list (cons (match-string 1) ahk-Keywords-list)))
      (forward-line 1))
    ;; read variables
    (erase-buffer)
    (insert-file-contents (expand-file-name "Variables.txt"
                                            ahk-syntax-directory))
    (setq ahk-Variables-list nil)
    (goto-char 0)
    (while (not (eobp))
      (if (not (looking-at "\\([^;\r\n][^\t\r\n]+\\)"))
          nil;; (error "Unknown file syntax of Variables.txt")
        (setq ahk-Variables-list (cons (match-string 1) ahk-Variables-list)))
      (forward-line 1))
  
    ;; built completion list
    (setq ahk-completion-list
          (mapcar (lambda (c) (list c))
                  (append (mapcar 'car ahk-Commands-list)
                          ahk-Keywords-list
                          ahk-Variables-list
                          ahk-Keys-list)))

    (setq ahk-mode-font-lock-keywords
          (list
           '("\\s-*;.*$" .
             font-lock-comment-face)
           '("^/\\*\\(.*\r?\n\\)*\\(\\*/\\)?" .
;           '(ahk-fontify-comment .
             font-lock-comment-face)
           '("^\\([^ \t\n:]+\\):" .
             (1 font-lock-builtin-face))
           '("[^, %\"]*%[^% ]+%" .
             font-lock-variable-name-face)
           ;; I get an error when using regexp-opt instead of simply
           ;; concatenating the keywords and I do not understand why ;-(
           ;; (warning/warning) Error caught in `font-lock-pre-idle-hook': (invalid-regexp Invalid preceding regular expression)
           (cons
            (concat "\\b\\("
                    (mapconcat 'regexp-quote ahk-Variables-list "\\|")
                    "\\)\\b")
            'font-lock-variable-name-face)
           (list
            (concat "\\(^[ \t]*\\|::[ \t]*\\)\\("
                    (mapconcat 'regexp-quote (mapcar 'car ahk-Commands-list) "\\|")
                    "\\)")
            2
            'font-lock-function-name-face)
           (cons
            (concat "\\b\\("
                    (mapconcat 'regexp-quote ahk-Keywords-list "\\|")
                    "\\)\\b")
            'font-lock-keyword-face)
           (cons
            (concat "\\b\\("
                    (mapconcat 'regexp-quote ahk-Keys-list "\\|")
                    "\\)\\b")
            'font-lock-constant-face)
           )))
  
  (message "Initializing ahk-mode variables done."))

;; this was an attempt to get multi-line comments correctly highlighted, I
;; tried to understand how cc-mode is doing it, but I have to admit I do not
;; understand it!
(defun ahk-fontify-comment (limit)
;  (setq limit (point-max))
  (save-excursion
    (let (start end)
;      (setq bstart (save-excursion
;                     (re-search-backward "^/\\*" (point-min) t)))
;      (setq bend (save-excursion
;                   (re-search-backward "^\\*/" (point-min) t)))
;      (if (and bstart bend (< bend bstart))
;          (goto-char bstart))
      (setq start (save-excursion
                    (re-search-forward "^/\\*" limit t)))
      (setq end (save-excursion
                  (re-search-forward "^\\*/" limit t)))
      
      (cond
       ((and start end (< end start))
        (save-excursion
          (re-search-forward "^\\*/" limit t)
          t))
       ((and start end (< start end))
        (save-excursion
          (re-search-forward "^/\\*\\(.*\r?\n\\)*\\*/" limit t)
          t))
       ((and start (not end))
        (save-excursion
          (re-search-forward "^/\\*\\(.*\r?\n\\)*" limit t)
          t))
       ))))

(defun ahk-mode-hook-activate-filling ()
  "Activates `auto-fill-mode' and `filladapt-mode'."
  (auto-fill-mode 1)
  (if (locate-library "filladapt")
      (filladapt-mode 1)))

;;;###autoload
(defun ahk-mode ()
  "Major mode for editing AutoHotKey Scripts.

The hook functions in `ahk-mode-hook' are run after mode initialization.

Key bindings:
\\{ahk-mode-map}"
  (interactive)
  (if (null ahk-Commands-list)
      (ahk-init))
  (kill-all-local-variables)
  (set-syntax-table ahk-mode-syntax-table)
  (setq major-mode 'ahk-mode
	mode-name "AHK"
	local-abbrev-table ahk-mode-abbrev-table
	abbrev-mode t
        indent-region-function 'ahk-indent-region)
  (put 'ahk-mode 'font-lock-defaults '(ahk-mode-font-lock-keywords t))
  (put 'ahk-mode 'font-lock-keywords-case-fold-search t)

  (when (not (featurep 'xemacs))
    (setq font-lock-defaults '(ahk-mode-font-lock-keywords))
    (setq font-lock-keywords-case-fold-search t))
  
  (use-local-map ahk-mode-map)
  (easy-menu-add ahk-menu)
  (setq comment-start ";")
  (font-lock-mode 1)
  (force-mode-line-update)
  (run-hooks 'ahk-mode-hook))

(defun ahk-indent-line ()
  "Indent the current line."
  (interactive)

  (let ((indent 0)
        (case-fold-search t))
    ;; do a backward search to determin the indention level
    (save-excursion
      (beginning-of-line)
      (if (looking-at "^;")
          (setq indent 0)
        (skip-chars-backward " \t\n")
        (beginning-of-line)
        (while (and (looking-at "^;") (not (bobp)))
          (forward-line -1))
        (if (looking-at "^[^: ]+:")
            (if (looking-at "^[^: ]+:\\([^:]*:\\)?[ \t]*$")
                (setq indent ahk-indetion)
              (setq indent 0))
          (if (looking-at "^\\([ \t]*\\)[{(]")
              (setq indent (+ (length (match-string 1)) ahk-indetion))
            (if (and
                 (save-excursion
                   (forward-line 1)
                   (not (looking-at "^\\([ \t]*\\)[{(]")))
                 (looking-at "^\\([ \t]*\\)\\(If\\|Else\\)"))
                (setq indent (+ (length (match-string 1)) ahk-indetion))
              (if (save-excursion
                    (forward-line -1)
                    (looking-at "^\\([ \t]*\\)\\(If\\|Else\\)"))
                  (setq indent (+ (length (match-string 1))))
                (if (looking-at "^\\([ \t]*\\)")
                    (setq indent (+ (length (match-string 1)))))))))))
    ;; check for special tokens
    (save-excursion
      (beginning-of-line)
      (if (looking-at "^\\([ \t]*\\)[})]")
          (setq indent (- indent ahk-indetion))
        (if (or (looking-at "^[ \t]*[^,: \t\n]*:")
                (looking-at "^;;;"))
            (setq indent 0))))
    (let ((p (point-marker)))
      (beginning-of-line)
      (if (looking-at "^[ \t]+")
          (replace-match ""))
      (indent-to indent)
      (goto-char p)
      (set-marker p nil)
      (if (bolp)
          (goto-char (+ (point) indent))))))

(defun ahk-indent-region (start end)
  "Indent lines in region START to END."
  (interactive "r")
  (save-excursion
    (goto-char end)
    (setq end (point-marker))
    (goto-char start)
    (while (< (point) end)
      (beginning-of-line)
      (ahk-indent-line)
      (forward-line 1))
    (ahk-indent-line)
    (set-marker end nil)))
  
(defun ahk-complete ()
  "Indent current line when at the beginning or complete current command."
  (interactive)

  (if (looking-at "\\w+")
      (goto-char (match-end 0)))
  
  (let ((end (point)))
    (if (and (or (save-excursion (re-search-backward "\\<\\w+"))
                 (looking-at "\\<\\w+"))
             (= (match-end 0) end))
        (let ((start (match-beginning 0))
              (prefix (match-string 0))
              (completion-ignore-case t)
              completions)
          (setq completions (all-completions prefix ahk-completion-list))
          (if (eq completions nil)
              nil;(error "Unknown command prefix <%s>!" prefix)
            (if (> (length completions) 1)
                (setq completions
                      (completing-read "Complete command: "
                                       (mapcar (lambda (c) (list c))
                                               completions)
                                       nil t prefix)))
            (if (stringp completions)
                ;; this is a trick to upcase "If" and other prefixes
                (let ((c (try-completion completions ahk-completion-list)))
                  (if (stringp c)
                      (setq completions c))))
            
            (delete-region start end)
            (if (listp completions) (setq completions (car completions)))
            (insert completions)
            (let ((help (assoc completions ahk-Commands-list)))
              (if help (message "%s" (mapconcat 'identity help ""))))
            )))))

(defun ahk-indent-line-and-complete ()
  "Combines indetion and completion."
  (interactive)
  (ahk-indent-line)
  (ahk-complete))

(defun ahk-electric-brace (arg)
  "Insert character ARG and correct line's indentation."
  (interactive "p")
  (if (save-excursion
        (skip-chars-backward " \t")
        (bolp))
      nil
    (ahk-indent-line)
    (newline))
  (self-insert-command arg)
  (ahk-indent-line)
  (newline)
  (ahk-indent-line)

  (let ((event  last-input-event))
    (setq event (if (featurep 'xemacs)
		    (event-to-character event)
		  (setq event (if (stringp event) (aref event 0) event))))

    (when (equal event ?{)
      (newline)
      (ahk-indent-line)
      (insert ?})
      (ahk-indent-line)
      (forward-line -1)
      (ahk-indent-line))))

(defun ahk-electric-return ()
  "Insert newline and indent."
  (interactive)
  (ahk-indent-line)
  (newline)
  (ahk-indent-line))

(defun ahk-insert-command-template ()
  "Insert a command template."
  (interactive)
  (let ((completions (mapcar (lambda (c) (list (mapconcat 'identity c "")))
                             ahk-Commands-list))
        (completion-ignore-case t)
        (start (point))
        end 
        command)
    (setq command (completing-read "AHK command template: " completions))
    (insert command)
    (ahk-indent-line)
    (setq end (point-marker))
    (goto-char start)
    (while (re-search-forward "[`][nt]" end t)
      (if (string= (match-string 0) "`n")
	  (replace-match "\n")
	(replace-match "")))
    (ahk-indent-region start end)
    (goto-char (1+ start))
    ;; jump to first parameter 
    (re-search-forward "\\<" end nil)
    (set-marker end nil)))

(defun ahk-comment-region (start end &optional arg)
  "Comment or uncomment each line in the region from START to END.
If no region is active use the current line."
  (interactive (if (region-active-p)
                   (list (region-beginning)
                         (region-end)
                         current-prefix-arg)
                 (let (start end)
                   (beginning-of-line)
                   (setq start (point))
                   (forward-line)
                   (setq end (point))
                   (list start end current-prefix-arg))))
  (save-excursion
    (comment-region start end arg)))

(defun ahk-www-help-at-point ()
  (interactive)
  (save-excursion
    (re-search-backward "\\<\\w+")
    (if (looking-at "\\<\\w+")
        (ahk-www-help (match-string 0)))))

(defvar ahk-www-help-alist '()
  "Mapping of command to regexp for commands which are not unique.")

(defun ahk-www-help (command)
  "Display online help for the given command"
  (interactive (list
                (completing-read "AHK command: "
                                 ahk-completion-list nil t)))
  (require 'w3)
  (w3-fetch "http://www.autohotkey.com/docs/commands.htm")
  (goto-char (point-min))
  (when (re-search-forward (concat "^|" (regexp-quote command)))
    (goto-char (+ (match-beginning 0) 1))
    (save-excursion
      (if (re-search-forward (concat "^|" (regexp-quote command))
                             (point-max) t)
          (message "There is more than one occurrence of %s. Stopping at first match" command)
        (widget-button-press (point))))))

(provide 'ahk-mode)

;;; ahk-mode.el ends here
