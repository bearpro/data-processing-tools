# Практическая работа №1

## Подсчитайте количество файлов и каталогов, расположенных в вашем домашнем каталоге

Количество файлов:

```bash
bearpro@dt-bearpro:~$ ls -la ~ | grep ^\- | wc
      7      63     429
```

Количество каталогов:

```bash
bearpro@dt-bearpro:~$ ls -la ~ | grep ^\d | wc
     11      99     648
```

## Отобразите содержимое файла только прописными буквами

```bash
bearpro@dt-bearpro:~$ echo LOL > tmp.txt
bearpro@dt-bearpro:~$ cat tmp.txt | awk '{print tolower($0)}'
lol
```

## Подсчитайте, сколько раз встречалось каждое слово в файле

```bash
bearpro@dt-bearpro:~$ echo LOL KEK LOL > tmp.txt
bearpro@dt-bearpro:~$ cat tmp.txt | tr ' ' '\n' | sort | uniq -c
      1 KEK
      2 LOL
```

## Подсчитайте количество гласных в файле

```bash
bearpro@dt-bearpro:~$ cat tmp.txt | grep -io '[aeiou]' | sort | uniq -c
      1 E
      2 O
```

### Отсортируйте результат от наиболее распространённой до наименее распространённой буквы

```bash
bearpro@dt-bearpro:~$ cat tmp.txt | grep -io '[aeiou]' | sort | uniq -c | sort -r
      2 O
      1 E
```
