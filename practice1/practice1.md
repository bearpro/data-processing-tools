# Практическая работа №1

## Подсчитайте количество файлов и каталогов, расположенных в вашем домашнем каталоге

Количество файлов:

```bash
bearpro@dt-bearpro:~$ ls -la ~ | grep ^\- | wc -l
7
```

Количество каталогов:

```bash
bearpro@dt-bearpro:~$ ls -la ~ | grep ^\d | wc -l
11
```

## Отобразите содержимое файла только прописными буквами

```bash
bearpro@dt-bearpro:~$ curl https://raw.githubusercontent.com/torvalds/linux/master/README >> tmp.txt
bearpro@dt-bearpro:~$ cat tmp.txt | awk '{print tolower($0)}'
linux kernel
============

there are several guides for kernel developers and users. these guides can
be rendered in a number of formats, like html and pdf. please read
documentation/admin-guide/readme.rst first.

in order to build the documentation, use ``make htmldocs`` or
``make pdfdocs``.  the formatted documentation can also be read online at:

    https://www.kernel.org/doc/html/latest/

there are various text files in the documentation/ subdirectory,
several of them using the restructured text markup notation.

please read the documentation/process/changes.rst file, as it contains the
requirements for building and running the kernel, and information about
the problems which may result by upgrading your kernel.
```

## Подсчитайте, сколько раз встречалось каждое слово в файле

```bash
bearpro@dt-bearpro:~$ cat tmp.txt | tr -s ',./:`= ' '\n' | sort | uniq -c
      3 Documentation
      1 HTML
      1 In
      1 Linux
      1 PDF
      2 Please
      1 README
      1 Restructured
      1 Text
      1 The
      2 There
      1 These
      1 a
      1 about
      1 admin-guide
      1 also
      4 and
      2 are
      1 as
      1 at
      2 be
      1 build
      1 building
      1 by
      2 can
      1 changes
      1 contains
      1 developers
      1 doc
      2 documentation
      1 file
      1 files
      1 first
      2 for
      1 formats
      1 formatted
      2 guides
      1 html
      1 htmldocs
      1 https
      2 in
      1 information
      1 it
      5 kernel
      1 latest
      1 like
      2 make
      1 markup
      1 may
      1 notation
      1 number
      2 of
      1 online
      1 or
      1 order
      1 org
      1 pdfdocs
      1 problems
      1 process
      3 read
      1 rendered
      1 requirements
      1 result
      2 rst
      1 running
      2 several
      1 subdirectory
      1 text
      7 the
      1 them
      1 to
      1 upgrading
      1 use
      1 users
      1 using
      1 various
      1 which
      1 www
      1 your
```

## Подсчитайте количество гласных в файле

```bash
bearpro@dt-bearpro:~$ cat tmp.txt | awk '{ print tolower($0) }'  | grep -io '[aeiou]' | sort | uniq -c
     40 a
     80 e
     33 i
     37 o
     26 u
```

### Отсортируйте результат от наиболее распространённой до наименее распространённой буквы

```bash
bearpro@dt-bearpro:~$ cat tmp.txt | awk '{ print tolower($0) }'  | grep -io '[aeiou]' | sort | uniq -c | sort -r
     80 e
     40 a
     37 o
     33 i
     26 u
```
