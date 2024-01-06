#!/usr/bin/python
import sys
import os

N1 = 1
N2 = 2
N3 = 3
N4 = 4
N5 = 5
N6 = 6
E1 = 7
E2 = 8
E3 = 9
E4 = 10
E5 = 11
E6 = 12
O1 = 13
O2 = 14
O3 = 15
O4 = 16
O5 = 17
O6 = 18
A1 = 19
A2 = 20
A3 = 21
A4 = 22
A5 = 23
A6 = 24
C1 = 25
C2 = 26
C3 = 27
C4 = 28
C5 = 29
C6 = 30
N = 31
E = 32
O = 33
A = 34
C = 35
Int = 36
Pow = 37
Cha = 38
gender = 39
age = 40

def processYoungFemale(array):
    result = list(array)
    #N
    if int(array[N]) >= 118:
        result[N] = "High"
    elif int(array[N]) >= 91:
        result[N] = "Medium"
    else:
        result[N] = "Low"
    #N1
    if int(array[N1]) >= 24:
        result[N1] = "High"
    elif int(array[N1]) >= 18:
        result[N1] = "Medium"
    else:
        result[N1] = "Low"
    #N2
    if int(array[N2]) >= 20:
        result[N2] = "High"
    elif int(array[N2]) >= 12:
        result[N2] = "Medium"
    else:
        result[N2] = "Low"
    #N3
    if int(array[N3]) >= 20:
        result[N3] = "High"
    elif int(array[N3]) >= 14:
        result[N3] = "Medium"
    else:
        result[N3] = "Low"
    #N4
    if int(array[N4]) >= 21:
        result[N4] = "High"
    elif int(array[N4]) >= 15:
        result[N4] = "Medium"
    else:
        result[N4] = "Low"
    #N5
    if int(array[N5]) >= 21:
        result[N5] = "High"
    elif int(array[N5]) >= 16:
        result[N5] = "Medium"
    else:
        result[N5] = "Low"
    #N6
    if int(array[N6]) >= 17:
        result[N6] = "High"
    elif int(array[N6]) >= 12:
        result[N6] = "Medium"
    else:
        result[N6] = "Low"
    #E
    if int(array[E]) >= 125:
        result[E] = "High"
    elif int(array[E]) >= 101:
        result[E] = "Medium"
    else:
        result[E] = "Low"
    #E1
    if int(array[E1]) >= 26:
        result[E1] = "High"
    elif int(array[E1]) >= 20:
        result[E1] = "Medium"
    else:
        result[E1] = "Low"
    #E2
    if int(array[E2]) >= 22:
        result[E2] = "High"
    elif int(array[E2]) >= 16:
        result[E2] = "Medium"
    else:
        result[E2] = "Low"
    #E3
    if int(array[E3]) >= 17:
        result[E3] = "High"
    elif int(array[E3]) >= 12:
        result[E3] = "Medium"
    else:
        result[E3] = "Low"
    #E4
    if int(array[E4]) >= 20:
        result[E4] = "High"
    elif int(array[E4]) >= 15:
        result[E4] = "Medium"
    else:
        result[E4] = "Low"
    #E5
    if int(array[E5]) >= 24:
        result[E5] = "High"
    elif int(array[E5]) >= 18:
        result[E5] = "Medium"
    else:
        result[E5] = "Low"
    #E6
    if int(array[E6]) >= 24:
        result[E6] = "High"
    elif int(array[E6]) >= 18:
        result[E6] = "Medium"
    else:
        result[E6] = "Low"
    #O
    if int(array[O]) >= 130:
        result[O] = "High"
    elif int(array[O]) >= 105:
        result[O] = "Medium"
    else:
        result[O] = "Low"
    #O1
    if int(array[O1]) >= 23:
        result[O1] = "High"
    elif int(array[O1]) >= 17:
        result[O1] = "Medium"
    else:
        result[O1] = "Low"
    #O2
    if int(array[O2]) >= 26:
        result[O2] = "High"
    elif int(array[O2]) >= 18:
        result[O2] = "Medium"
    else:
        result[O2] = "Low"
    #O3
    if int(array[O3]) >= 24:
        result[O3] = "High"
    elif int(array[O3]) >= 19:
        result[O3] = "Medium"
    else:
        result[O3] = "Low"
    #O4
    if int(array[O4]) >= 20:
        result[O4] = "High"
    elif int(array[O4]) >= 16:
        result[O4] = "Medium"
    else:
        result[O4] = "Low"
    #O5
    if int(array[O5]) >= 22:
        result[O5] = "High"
    elif int(array[O5]) >= 16:
        result[O5] = "Medium"
    else:
        result[O5] = "Low"
    #O6
    if int(array[O6]) >= 21:
        result[O6] = "High"
    elif int(array[O6]) >= 16:
        result[O6] = "Medium"
    else:
        result[O6] = "Low"
    #A
    if int(array[A]) >= 128:
        result[A] = "High"
    elif int(array[A]) >= 108:
        result[A] = "Medium"
    else:
        result[A] = "Low"
    #A1
    if int(array[A1]) >= 21:
        result[A1] = "High"
    elif int(array[A1]) >= 16:
        result[A1] = "Medium"
    else:
        result[A1] = "Low"
    #A2
    if int(array[A2]) >= 21:
        result[A2] = "High"
    elif int(array[A2]) >= 16:
        result[A2] = "Medium"
    else:
        result[A2] = "Low"
    #A3
    if int(array[A3]) >= 25:
        result[A3] = "High"
    elif int(array[A3]) >= 21:
        result[A3] = "Medium"
    else:
        result[A3] = "Low"
    #A4
    if int(array[A4]) >= 21:
        result[A4] = "High"
    elif int(array[A4]) >= 15:
        result[A4] = "Medium"
    else:
        result[A4] = "Low"
    #A5
    if int(array[A5]) >= 23:
        result[A5] = "High"
    elif int(array[A5]) >= 17:
        result[A5] = "Medium"
    else:
        result[A5] = "Low"
    #A6
    if int(array[A6]) >= 24:
        result[A6] = "High"
    elif int(array[A6]) >= 20:
        result[A6] = "Medium"
    else:
        result[A6] = "Low"
    #C
    if int(array[C]) >= 127:
        result[C] = "High"
    elif int(array[C]) >= 103:
        result[C] = "Medium"
    else:
        result[C] = "Low"
    #C1
    if int(array[C1]) >= 22:
        result[C1] = "High"
    elif int(array[C1]) >= 18:
        result[C1] = "Medium"
    else:
        result[C1] = "Low"
    #C2
    if int(array[C2]) >= 23:
        result[C2] = "High"
    elif int(array[C2]) >= 17:
        result[C2] = "Medium"
    else:
        result[C2] = "Low"
    #C3
    if int(array[C3]) >= 25:
        result[C3] = "High"
    elif int(array[C3]) >= 19:
        result[C3] = "Medium"
    else:
        result[C3] = "Low"
    #C4
    if int(array[C4]) >= 22:
        result[C4] = "High"
    elif int(array[C4]) >= 18:
        result[C4] = "Medium"
    else:
        result[C4] = "Low"
    #C5
    if int(array[C5]) >= 21:
        result[C5] = "High"
    elif int(array[C5]) >= 16:
        result[C5] = "Medium"
    else:
        result[C5] = "Low"
    #C6
    if int(array[C6]) >= 20:
        result[C6] = "High"
    elif int(array[C6]) >= 14:
        result[C6] = "Medium"
    else:
        result[C6] = "Low"
    #Internal
    if int(array[Int]) >= 36.75:
        result[Int] = "High"
    elif int(array[Int]) >= 29:
        result[Int] = "Medium"
    else:
        result[Int] = "Low"
    #Powerful Others
    if int(array[Pow]) >= 20.75:
        result[Pow] = "High"
    elif int(array[Pow]) >= 12:
        result[Pow] = "Medium"
    else:
        result[Pow] = "Low"
    #Chance
    if int(array[Cha]) >= 21.75:
        result[Cha] = "High"
    elif int(array[Cha]) >= 14:
        result[Cha] = "Medium"
    else:
        result[Cha] = "Low"
    return result[:len(result)-2]

def processAdultFemale(array):
    result = list(array)
    #N
    if int(array[N]) >= 113:
        result[N] = "High"
    elif int(array[N]) >= 86:
        result[N] = "Medium"
    else:
        result[N] = "Low"
    #N1
    if int(array[N1]) >= 24:
        result[N1] = "High"
    elif int(array[N1]) >= 18:
        result[N1] = "Medium"
    else:
        result[N1] = "Low"
    #N2
    if int(array[N2]) >= 18:
        result[N2] = "High"
    elif int(array[N2]) >= 12:
        result[N2] = "Medium"
    else:
        result[N2] = "Low"
    #N3
    if int(array[N3]) >= 21:
        result[N3] = "High"
    elif int(array[N3]) >= 14:
        result[N3] = "Medium"
    else:
        result[N3] = "Low"
    #N4
    if int(array[N4]) >= 20:
        result[N4] = "High"
    elif int(array[N4]) >= 14:
        result[N4] = "Medium"
    else:
        result[N4] = "Low"
    #N5
    if int(array[N5]) >= 19:
        result[N5] = "High"
    elif int(array[N5]) >= 14:
        result[N5] = "Medium"
    else:
        result[N5] = "Low"
    #N6
    if int(array[N6]) >= 17:
        result[N6] = "High"
    elif int(array[N6]) >= 11:
        result[N6] = "Medium"
    else:
        result[N6] = "Low"
    #E
    if int(array[E]) >= 115:
        result[E] = "High"
    elif int(array[E]) >= 93:
        result[E] = "Medium"
    else:
        result[E] = "Low"
    #E1
    if int(array[E1]) >= 25:
        result[E1] = "High"
    elif int(array[E1]) >= 20:
        result[E1] = "Medium"
    else:
        result[E1] = "Low"
    #E2
    if int(array[E2]) >= 20:
        result[E2] = "High"
    elif int(array[E2]) >= 14:
        result[E2] = "Medium"
    else:
        result[E2] = "Low"
    #E3
    if int(array[E3]) >= 16:
        result[E3] = "High"
    elif int(array[E3]) >= 11:
        result[E3] = "Medium"
    else:
        result[E3] = "Low"
    #E4
    if int(array[E4]) >= 20:
        result[E4] = "High"
    elif int(array[E4]) >= 15:
        result[E4] = "Medium"
    else:
        result[E4] = "Low"
    #E5
    if int(array[E5]) >= 20:
        result[E5] = "High"
    elif int(array[E5]) >= 14:
        result[E5] = "Medium"
    else:
        result[E5] = "Low"
    #E6
    if int(array[E6]) >= 21:
        result[E6] = "High"
    elif int(array[E6]) >= 16:
        result[E6] = "Medium"
    else:
        result[E6] = "Low"
    #O
    if int(array[O]) >= 121:
        result[O] = "High"
    elif int(array[O]) >= 95:
        result[O] = "Medium"
    else:
        result[O] = "Low"
    #O1
    if int(array[O1]) >= 21:
        result[O1] = "High"
    elif int(array[O1]) >= 14:
        result[O1] = "Medium"
    else:
        result[O1] = "Low"
    #O2
    if int(array[O2]) >= 24:
        result[O2] = "High"
    elif int(array[O2]) >= 17:
        result[O2] = "Medium"
    else:
        result[O2] = "Low"
    #O3
    if int(array[O3]) >= 22:
        result[O3] = "High"
    elif int(array[O3]) >= 17:
        result[O3] = "Medium"
    else:
        result[O3] = "Low"
    #O4
    if int(array[O4]) >= 19:
        result[O4] = "High"
    elif int(array[O4]) >= 14:
        result[O4] = "Medium"
    else:
        result[O4] = "Low"
    #O5
    if int(array[O5]) >= 20:
        result[O5] = "High"
    elif int(array[O5]) >= 13:
        result[O5] = "Medium"
    else:
        result[O5] = "Low"
    #O6
    if int(array[O6]) >= 20:
        result[O6] = "High"
    elif int(array[O6]) >= 16:
        result[O6] = "Medium"
    else:
        result[O6] = "Low"
    #A
    if int(array[A]) >= 134:
        result[A] = "High"
    elif int(array[A]) >= 112:
        result[A] = "Medium"
    else:
        result[A] = "Low"
    #A1
    if int(array[A1]) >= 22:
        result[A1] = "High"
    elif int(array[A1]) >= 16:
        result[A1] = "Medium"
    else:
        result[A1] = "Low"
    #A2
    if int(array[A2]) >= 23:
        result[A2] = "High"
    elif int(array[A2]) >= 17:
        result[A2] = "Medium"
    else:
        result[A2] = "Low"
    #A3
    if int(array[A3]) >= 25:
        result[A3] = "High"
    elif int(array[A3]) >= 20:
        result[A3] = "Medium"
    else:
        result[A3] = "Low"
    #A4
    if int(array[A4]) >= 23:
        result[A4] = "High"
    elif int(array[A4]) >= 17:
        result[A4] = "Medium"
    else:
        result[A4] = "Low"
    #A5
    if int(array[A5]) >= 24:
        result[A5] = "High"
    elif int(array[A5]) >= 18:
        result[A5] = "Medium"
    else:
        result[A5] = "Low"
    #A6
    if int(array[A6]) >= 24:
        result[A6] = "High"
    elif int(array[A6]) >= 20:
        result[A6] = "Medium"
    else:
        result[A6] = "Low"
    #C
    if int(array[C]) >= 134:
        result[C] = "High"
    elif int(array[C]) >= 106:
        result[C] = "Medium"
    else:
        result[C] = "Low"
    #C1
    if int(array[C1]) >= 23:
        result[C1] = "High"
    elif int(array[C1]) >= 18:
        result[C1] = "Medium"
    else:
        result[C1] = "Low"
    #C2
    if int(array[C2]) >= 23:
        result[C2] = "High"
    elif int(array[C2]) >= 17:
        result[C2] = "Medium"
    else:
        result[C2] = "Low"
    #C3
    if int(array[C3]) >= 26:
        result[C3] = "High"
    elif int(array[C3]) >= 21:
        result[C3] = "Medium"
    else:
        result[C3] = "Low"
    #C4
    if int(array[C4]) >= 23:
        result[C4] = "High"
    elif int(array[C4]) >= 18:
        result[C4] = "Medium"
    else:
        result[C4] = "Low"
    #C5
    if int(array[C5]) >= 22:
        result[C5] = "High"
    elif int(array[C5]) >= 17:
        result[C5] = "Medium"
    else:
        result[C5] = "Low"
    #C6
    if int(array[C6]) >= 22:
        result[C6] = "High"
    elif int(array[C6]) >= 15:
        result[C6] = "Medium"
    else:
        result[C6] = "Low"
    #Internal
    if int(array[Int]) >= 36.75:
        result[Int] = "High"
    elif int(array[Int]) >= 29:
        result[Int] = "Medium"
    else:
        result[Int] = "Low"
    #Powerful Others
    if int(array[Pow]) >= 20.75:
        result[Pow] = "High"
    elif int(array[Pow]) >= 12:
        result[Pow] = "Medium"
    else:
        result[Pow] = "Low"
    #Chance
    if int(array[Cha]) >= 21.75:
        result[Cha] = "High"
    elif int(array[Cha]) >= 14:
        result[Cha] = "Medium"
    else:
        result[Cha] = "Low"
    return result[:len(result)-2]

def processYoungMale(array):
    result = list(array)
    #N
    if int(array[N]) >= 108:
        result[N] = "High"
    elif int(array[N]) >= 86:
        result[N] = "Medium"
    else:
        result[N] = "Low"
    #N1
    if int(array[N1]) >= 21:
        result[N1] = "High"
    elif int(array[N1]) >= 15:
        result[N1] = "Medium"
    else:
        result[N1] = "Low"
    #N2
    if int(array[N2]) >= 17:
        result[N2] = "High"
    elif int(array[N2]) >= 12:
        result[N2] = "Medium"
    else:
        result[N2] = "Low"
    #N3
    if int(array[N3]) >= 19:
        result[N3] = "High"
    elif int(array[N3]) >= 13:
        result[N3] = "Medium"
    else:
        result[N3] = "Low"
    #N4
    if int(array[N4]) >= 19:
        result[N4] = "High"
    elif int(array[N4]) >= 14:
        result[N4] = "Medium"
    else:
        result[N4] = "Low"
    #N5
    if int(array[N5]) >= 20:
        result[N5] = "High"
    elif int(array[N5]) >= 16:
        result[N5] = "Medium"
    else:
        result[N5] = "Low"
    #N6
    if int(array[N6]) >= 16:
        result[N6] = "High"
    elif int(array[N6]) >= 11:
        result[N6] = "Medium"
    else:
        result[N6] = "Low"
    #E
    if int(array[E]) >= 123:
        result[E] = "High"
    elif int(array[E]) >= 102:
        result[E] = "Medium"
    else:
        result[E] = "Low"
    #E1
    if int(array[E1]) >= 24:
        result[E1] = "High"
    elif int(array[E1]) >= 19:
        result[E1] = "Medium"
    else:
        result[E1] = "Low"
    #E2
    if int(array[E2]) >= 21:
        result[E2] = "High"
    elif int(array[E2]) >= 14:
        result[E2] = "Medium"
    else:
        result[E2] = "Low"
    #E3
    if int(array[E3]) >= 18:
        result[E3] = "High"
    elif int(array[E3]) >= 13:
        result[E3] = "Medium"
    else:
        result[E3] = "Low"
    #E4
    if int(array[E4]) >= 19:
        result[E4] = "High"
    elif int(array[E4]) >= 15:
        result[E4] = "Medium"
    else:
        result[E4] = "Low"
    #E5
    if int(array[E5]) >= 24:
        result[E5] = "High"
    elif int(array[E5]) >= 19:
        result[E5] = "Medium"
    else:
        result[E5] = "Low"
    #E6
    if int(array[E6]) >= 23:
        result[E6] = "High"
    elif int(array[E6]) >= 18:
        result[E6] = "Medium"
    else:
        result[E6] = "Low"
    #O
    if int(array[O]) >= 121:
        result[O] = "High"
    elif int(array[O]) >= 99:
        result[O] = "Medium"
    else:
        result[O] = "Low"
    #O1
    if int(array[O1]) >= 21:
        result[O1] = "High"
    elif int(array[O1]) >= 15:
        result[O1] = "Medium"
    else:
        result[O1] = "Low"
    #O2
    if int(array[O2]) >= 23:
        result[O2] = "High"
    elif int(array[O2]) >= 17:
        result[O2] = "Medium"
    else:
        result[O2] = "Low"
    #O3
    if int(array[O3]) >= 23:
        result[O3] = "High"
    elif int(array[O3]) >= 18:
        result[O3] = "Medium"
    else:
        result[O3] = "Low"
    #O4
    if int(array[O4]) >= 19:
        result[O4] = "High"
    elif int(array[O4]) >= 15:
        result[O4] = "Medium"
    else:
        result[O4] = "Low"
    #O5
    if int(array[O5]) >= 22:
        result[O5] = "High"
    elif int(array[O5]) >= 15:
        result[O5] = "Medium"
    else:
        result[O5] = "Low"
    #O6
    if int(array[O6]) >= 21:
        result[O6] = "High"
    elif int(array[O6]) >= 16:
        result[O6] = "Medium"
    else:
        result[O6] = "Low"
    #A
    if int(array[A]) >= 123:
        result[A] = "High"
    elif int(array[A]) >= 100:
        result[A] = "Medium"
    else:
        result[A] = "Low"
    #A1
    if int(array[A1]) >= 21:
        result[A1] = "High"
    elif int(array[A1]) >= 15:
        result[A1] = "Medium"
    else:
        result[A1] = "Low"
    #A2
    if int(array[A2]) >= 19:
        result[A2] = "High"
    elif int(array[A2]) >= 14:
        result[A2] = "Medium"
    else:
        result[A2] = "Low"
    #A3
    if int(array[A3]) >= 23:
        result[A3] = "High"
    elif int(array[A3]) >= 18:
        result[A3] = "Medium"
    else:
        result[A3] = "Low"
    #A4
    if int(array[A4]) >= 21:
        result[A4] = "High"
    elif int(array[A4]) >= 15:
        result[A4] = "Medium"
    else:
        result[A4] = "Low"
    #A5
    if int(array[A5]) >= 21:
        result[A5] = "High"
    elif int(array[A5]) >= 16:
        result[A5] = "Medium"
    else:
        result[A5] = "Low"
    #A6
    if int(array[A6]) >= 23:
        result[A6] = "High"
    elif int(array[A6]) >= 18:
        result[A6] = "Medium"
    else:
        result[A6] = "Low"
    #C
    if int(array[C]) >= 126:
        result[C] = "High"
    elif int(array[C]) >= 98:
        result[C] = "Medium"
    else:
        result[C] = "Low"
    #C1
    if int(array[C1]) >= 22:
        result[C1] = "High"
    elif int(array[C1]) >= 17:
        result[C1] = "Medium"
    else:
        result[C1] = "Low"
    #C2
    if int(array[C2]) >= 22:
        result[C2] = "High"
    elif int(array[C2]) >= 16:
        result[C2] = "Medium"
    else:
        result[C2] = "Low"
    #C3
    if int(array[C3]) >= 23:
        result[C3] = "High"
    elif int(array[C3]) >= 18:
        result[C3] = "Medium"
    else:
        result[C3] = "Low"
    #C4
    if int(array[C4]) >= 23:
        result[C4] = "High"
    elif int(array[C4]) >= 17:
        result[C4] = "Medium"
    else:
        result[C4] = "Low"
    #C5
    if int(array[C5]) >= 21:
        result[C5] = "High"
    elif int(array[C5]) >= 16:
        result[C5] = "Medium"
    else:
        result[C5] = "Low"
    #C6
    if int(array[C6]) >= 20:
        result[C6] = "High"
    elif int(array[C6]) >= 14:
        result[C6] = "Medium"
    else:
        result[C6] = "Low"
    #Internal
    if int(array[Int]) >= 36.75:
        result[Int] = "High"
    elif int(array[Int]) >= 29:
        result[Int] = "Medium"
    else:
        result[Int] = "Low"
    #Powerful Others
    if int(array[Pow]) >= 20.75:
        result[Pow] = "High"
    elif int(array[Pow]) >= 12:
        result[Pow] = "Medium"
    else:
        result[Pow] = "Low"
    #Chance
    if int(array[Cha]) >= 21.75:
        result[Cha] = "High"
    elif int(array[Cha]) >= 14:
        result[Cha] = "Medium"
    else:
        result[Cha] = "Low"
    return result[:len(result)-2]

def processAdultMale(array):
    result = list(array)
    #N
    if int(array[N]) >= 104:
        result[N] = "High"
    elif int(array[N]) >= 79:
        result[N] = "Medium"
    else:
        result[N] = "Low"
    #N1
    if int(array[N1]) >= 21:
        result[N1] = "High"
    elif int(array[N1]) >= 15:
        result[N1] = "Medium"
    else:
        result[N1] = "Low"
    #N2
    if int(array[N2]) >= 17:
        result[N2] = "High"
    elif int(array[N2]) >= 11:
        result[N2] = "Medium"
    else:
        result[N2] = "Low"
    #N3
    if int(array[N3]) >= 19:
        result[N3] = "High"
    elif int(array[N3]) >= 12:
        result[N3] = "Medium"
    else:
        result[N3] = "Low"
    #N4
    if int(array[N4]) >= 19:
        result[N4] = "High"
    elif int(array[N4]) >= 13:
        result[N4] = "Medium"
    else:
        result[N4] = "Low"
    #N5
    if int(array[N5]) >= 19:
        result[N5] = "High"
    elif int(array[N5]) >= 14:
        result[N5] = "Medium"
    else:
        result[N5] = "Low"
    #N6
    if int(array[N6]) >= 15:
        result[N6] = "High"
    elif int(array[N6]) >= 9:
        result[N6] = "Medium"
    else:
        result[N6] = "Low"
    #E
    if int(array[E]) >= 120:
        result[E] = "High"
    elif int(array[E]) >= 98:
        result[E] = "Medium"
    else:
        result[E] = "Low"
    #E1
    if int(array[E1]) >= 25:
        result[E1] = "High"
    elif int(array[E1]) >= 19:
        result[E1] = "Medium"
    else:
        result[E1] = "Low"
    #E2
    if int(array[E2]) >= 20:
        result[E2] = "High"
    elif int(array[E2]) >= 14:
        result[E2] = "Medium"
    else:
        result[E2] = "Low"
    #E3
    if int(array[E3]) >= 18:
        result[E3] = "High"
    elif int(array[E3]) >= 12:
        result[E3] = "Medium"
    else:
        result[E3] = "Low"
    #E4
    if int(array[E4]) >= 20:
        result[E4] = "High"
    elif int(array[E4]) >= 15:
        result[E4] = "Medium"
    else:
        result[E4] = "Low"
    #E5
    if int(array[E5]) >= 23:
        result[E5] = "High"
    elif int(array[E5]) >= 16:
        result[E5] = "Medium"
    else:
        result[E5] = "Low"
    #E6
    if int(array[E6]) >= 22:
        result[E6] = "High"
    elif int(array[E6]) >= 16:
        result[E6] = "Medium"
    else:
        result[E6] = "Low"
    #O
    if int(array[O]) >= 122:
        result[O] = "High"
    elif int(array[O]) >= 95:
        result[O] = "Medium"
    else:
        result[O] = "Low"
    #O1
    if int(array[O1]) >= 20:
        result[O1] = "High"
    elif int(array[O1]) >= 14:
        result[O1] = "Medium"
    else:
        result[O1] = "Low"
    #O2
    if int(array[O2]) >= 23:
        result[O2] = "High"
    elif int(array[O2]) >= 16:
        result[O2] = "Medium"
    else:
        result[O2] = "Low"
    #O3
    if int(array[O3]) >= 23:
        result[O3] = "High"
    elif int(array[O3]) >= 17:
        result[O3] = "Medium"
    else:
        result[O3] = "Low"
    #O4
    if int(array[O4]) >= 19:
        result[O4] = "High"
    elif int(array[O4]) >= 14:
        result[O4] = "Medium"
    else:
        result[O4] = "Low"
    #O5
    if int(array[O5]) >= 22:
        result[O5] = "High"
    elif int(array[O5]) >= 15:
        result[O5] = "Medium"
    else:
        result[O5] = "Low"
    #O6
    if int(array[O6]) >= 20:
        result[O6] = "High"
    elif int(array[O6]) >= 16:
        result[O6] = "Medium"
    else:
        result[O6] = "Low"
    #A
    if int(array[A]) >= 128:
        result[A] = "High"
    elif int(array[A]) >= 106:
        result[A] = "Medium"
    else:
        result[A] = "Low"
    #A1
    if int(array[A1]) >= 22:
        result[A1] = "High"
    elif int(array[A1]) >= 16:
        result[A1] = "Medium"
    else:
        result[A1] = "Low"
    #A2
    if int(array[A2]) >= 22:
        result[A2] = "High"
    elif int(array[A2]) >= 16:
        result[A2] = "Medium"
    else:
        result[A2] = "Low"
    #A3
    if int(array[A3]) >= 24:
        result[A3] = "High"
    elif int(array[A3]) >= 19:
        result[A3] = "Medium"
    else:
        result[A3] = "Low"
    #A4
    if int(array[A4]) >= 22:
        result[A4] = "High"
    elif int(array[A4]) >= 16:
        result[A4] = "Medium"
    else:
        result[A4] = "Low"
    #A5
    if int(array[A5]) >= 23:
        result[A5] = "High"
    elif int(array[A5]) >= 17:
        result[A5] = "Medium"
    else:
        result[A5] = "Low"
    #A6
    if int(array[A6]) >= 24:
        result[A6] = "High"
    elif int(array[A6]) >= 19:
        result[A6] = "Medium"
    else:
        result[A6] = "Low"
    #C
    if int(array[C]) >= 134:
        result[C] = "High"
    elif int(array[C]) >= 106:
        result[C] = "Medium"
    else:
        result[C] = "Low"
    #C1
    if int(array[C1]) >= 23:
        result[C1] = "High"
    elif int(array[C1]) >= 18:
        result[C1] = "Medium"
    else:
        result[C1] = "Low"
    #C2
    if int(array[C2]) >= 22:
        result[C2] = "High"
    elif int(array[C2]) >= 16:
        result[C2] = "Medium"
    else:
        result[C2] = "Low"
    #C3
    if int(array[C3]) >= 26:
        result[C3] = "High"
    elif int(array[C3]) >= 20:
        result[C3] = "Medium"
    else:
        result[C3] = "Low"
    #C4
    if int(array[C4]) >= 23:
        result[C4] = "High"
    elif int(array[C4]) >= 18:
        result[C4] = "Medium"
    else:
        result[C4] = "Low"
    #C5
    if int(array[C5]) >= 23:
        result[C5] = "High"
    elif int(array[C5]) >= 17:
        result[C5] = "Medium"
    else:
        result[C5] = "Low"
    #C6
    if int(array[C6]) >= 22:
        result[C6] = "High"
    elif int(array[C6]) >= 15:
        result[C6] = "Medium"
    else:
        result[C6] = "Low"
    #Internal
    if int(array[Int]) >= 36.75:
        result[Int] = "High"
    elif int(array[Int]) >= 29:
        result[Int] = "Medium"
    else:
        result[Int] = "Low"
    #Powerful Others
    if int(array[Pow]) >= 20.75:
        result[Pow] = "High"
    elif int(array[Pow]) >= 12:
        result[Pow] = "Medium"
    else:
        result[Pow] = "Low"
    #Chance
    if int(array[Cha]) >= 21.75:
        result[Cha] = "High"
    elif int(array[Cha]) >= 14:
        result[Cha] = "Medium"
    else:
        result[Cha] = "Low"
    return result[:len(result)-2]

def processThreeParts(array):
    #print(array)
    if array[gender] == "Feminino":
        if array[age] <= 21:
            return processYoungFemale(array)
        else:
            return processAdultFemale(array)
    elif array[gender] == "Masculino":
        if array[age] <= 21:
            return processYoungMale(array)
        else:
            return processAdultMale(array)
    else:
        print("Error in gender for participant with id = " + str(array[0]))