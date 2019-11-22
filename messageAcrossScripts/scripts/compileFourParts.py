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

def processFourParts(array):
    #print(array)
    result = list(array)
    #N
    if int(array[N]) >= 100:
        result[N] = "High"
    elif int(array[N]) >= 80:
        result[N] = "Medium_High"
    elif int(array[N]) >= 80:
        result[N] = "Medium_Low"
    else:
        result[N] = "Low"
    #N1
    if int(array[N1]) >= 21:
        result[N1] = "High"
    elif int(array[N1]) >= 19:
        result[N1] = "Medium_High"
    elif int(array[N1]) >= 16:
        result[N1] = "Medium_Low"
    else:
        result[N1] = "Low"
    #N2
    if int(array[N2]) >= 17:
        result[N2] = "High"
    elif int(array[N2]) >= 15:
        result[N2] = "Medium_High"
    elif int(array[N2]) >= 12:
        result[N2] = "Medium_Low"
    else:
        result[N2] = "Low"
    #N3
    if int(array[N3]) >= 18:
        result[N3] = "High"
    elif int(array[N3]) >= 16:
        result[N3] = "Medium_High"
    elif int(array[N3]) >= 14:
        result[N3] = "Medium_Low"
    else:
        result[N3] = "Low"
    #N4
    if int(array[N4]) >= 18:
        result[N4] = "High"
    elif int(array[N4]) >= 15:
        result[N4] = "Medium_High"
    elif int(array[N4]) >= 14:
        result[N4] = "Medium_Low"
    else:
        result[N4] = "Low"
    #N5
    if int(array[N5]) >= 16:
        result[N5] = "High"
    elif int(array[N5]) >= 15:
        result[N5] = "Medium_High"
    elif int(array[N5]) >= 13:
        result[N5] = "Medium_Low"
    else:
        result[N5] = "Low"
    #N6
    if int(array[N6]) >= 15:
        result[N6] = "High"
    elif int(array[N6]) >= 12:
        result[N6] = "Medium_High"
    elif int(array[N6]) >= 10:
        result[N6] = "Medium_Low"
    else:
        result[N6] = "Low"
    #E
    if int(array[E]) >= 113:
        result[E] = "High"
    elif int(array[E]) >= 96:
        result[E] = "Medium_High"
    elif int(array[E]) >= 80:
        result[E] = "Medium_Low"
    else:
        result[E] = "Low"
    #E1
    if int(array[E1]) >= 24:
        result[E1] = "High"
    elif int(array[E1]) >= 22:
        result[E1] = "Medium_High"
    elif int(array[E1]) >= 20:
        result[E1] = "Medium_Low"
    else:
        result[E1] = "Low"
    #E2
    if int(array[E2]) >= 19:
        result[E2] = "High"
    elif int(array[E2]) >= 17:
        result[E2] = "Medium_High"
    elif int(array[E2]) >= 15:
        result[E2] = "Medium_Low"
    else:
        result[E2] = "Low"
    #E3
    if int(array[E3]) >= 17:
        result[E3] = "High"
    elif int(array[E3]) >= 15:
        result[E3] = "Medium_High"
    elif int(array[E3]) >= 12:
        result[E3] = "Medium_Low"
    else:
        result[E3] = "Low"
    #E4
    if int(array[E4]) >= 19:
        result[E4] = "High"
    elif int(array[E4]) >= 17:
        result[E4] = "Medium_High"
    elif int(array[E4]) >= 16:
        result[E4] = "Medium_Low"
    else:
        result[E4] = "Low"
    #E5
    if int(array[E5]) >= 20:
        result[E5] = "High"
    elif int(array[E5]) >= 18:
        result[E5] = "Medium_High"
    elif int(array[E5]) >= 16:
        result[E5] = "Medium_Low"
    else:
        result[E5] = "Low"
    #E6
    if int(array[E6]) >= 20:
        result[E6] = "High"
    elif int(array[E6]) >= 19:
        result[E6] = "Medium_High"
    elif int(array[E6]) >= 17:
        result[E6] = "Medium_Low"
    else:
        result[E6] = "Low"
    #O
    if int(array[O]) >= 115:
        result[O] = "High"
    elif int(array[O]) >= 96:
        result[O] = "Medium_High"
    elif int(array[O]) >= 80:
        result[O] = "Medium_Low"
    else:
        result[O] = "Low"
    #O1
    if int(array[O1]) >= 19:
        result[O1] = "High"
    elif int(array[O1]) >= 17:
        result[O1] = "Medium_High"
    elif int(array[O1]) >= 14:
        result[O1] = "Medium_Low"
    else:
        result[O1] = "Low"
    #O2
    if int(array[O2]) >= 23:
        result[O2] = "High"
    elif int(array[O2]) >= 20:
        result[O2] = "Medium_High"
    elif int(array[O2]) >= 17:
        result[O2] = "Medium_Low"
    else:
        result[O2] = "Low"
    #O3
    if int(array[O3]) >= 21:
        result[O3] = "High"
    elif int(array[O3]) >= 20:
        result[O3] = "Medium_High"
    elif int(array[O3]) >= 18:
        result[O3] = "Medium_Low"
    else:
        result[O3] = "Low"
    #O4
    if int(array[O4]) >= 18:
        result[O4] = "High"
    elif int(array[O4]) >= 17:
        result[O4] = "Medium_High"
    elif int(array[O4]) >= 15:
        result[O4] = "Medium_Low"
    else:
        result[O4] = "Low"
    #O5
    if int(array[O5]) >= 20:
        result[O5] = "High"
    elif int(array[O5]) >= 16:
        result[O5] = "Medium_High"
    elif int(array[O5]) >= 15:
        result[O5] = "Medium_Low"
    else:
        result[O5] = "Low"
    #O6
    if int(array[O6]) >= 19:
        result[O6] = "High"
    elif int(array[O6]) >= 17:
        result[O6] = "Medium_High"
    elif int(array[O6]) >= 16:
        result[O6] = "Medium_Low"
    else:
        result[O6] = "Low"
    #A
    if int(array[A]) >= 130:
        result[A] = "High"
    elif int(array[A]) >= 113:
        result[A] = "Medium_High"
    elif int(array[A]) >= 80:
        result[A] = "Medium_Low"
    else:
        result[A] = "Low"
    #A1
    if int(array[A1]) >= 22:
        result[A1] = "High"
    elif int(array[A1]) >= 19:
        result[A1] = "Medium_High"
    elif int(array[A1]) >= 17:
        result[A1] = "Medium_Low"
    else:
        result[A1] = "Low"
    #A2
    if int(array[A2]) >= 22:
        result[A2] = "High"
    elif int(array[A2]) >= 20:
        result[A2] = "Medium_High"
    elif int(array[A2]) >= 17:
        result[A2] = "Medium_Low"
    else:
        result[A2] = "Low"
    #A3
    if int(array[A3]) >= 25:
        result[A3] = "High"
    elif int(array[A3]) >= 23:
        result[A3] = "Medium_High"
    elif int(array[A3]) >= 21:
        result[A3] = "Medium_Low"
    else:
        result[A3] = "Low"
    #A4
    if int(array[A4]) >= 23:
        result[A4] = "High"
    elif int(array[A4]) >= 21:
        result[A4] = "Medium_High"
    elif int(array[A4]) >= 18:
        result[A4] = "Medium_Low"
    else:
        result[A4] = "Low"
    #A5
    if int(array[A5]) >= 23:
        result[A5] = "High"
    elif int(array[A5]) >= 21:
        result[A5] = "Medium_High"
    elif int(array[A5]) >= 19:
        result[A5] = "Medium_Low"
    else:
        result[A5] = "Low"
    #A6
    if int(array[A6]) >= 24:
        result[A6] = "High"
    elif int(array[A6]) >= 22:
        result[A6] = "Medium_High"
    elif int(array[A6]) >= 20:
        result[A6] = "Medium_Low"
    else:
        result[A6] = "Low"
    #C
    if int(array[C]) >= 135:
        result[C] = "High"
    elif int(array[C]) >= 124:
        result[C] = "Medium_High"
    elif int(array[C]) >= 80:
        result[C] = "Medium_Low"
    else:
        result[C] = "Low"
    #C1
    if int(array[C1]) >= 23:
        result[C1] = "High"
    elif int(array[C1]) >= 21:
        result[C1] = "Medium_High"
    elif int(array[C1]) >= 20:
        result[C1] = "Medium_Low"
    else:
        result[C1] = "Low"
    #C2
    if int(array[C2]) >= 23:
        result[C2] = "High"
    elif int(array[C2]) >= 21:
        result[C2] = "Medium_High"
    elif int(array[C2]) >= 18:
        result[C2] = "Medium_Low"
    else:
        result[C2] = "Low"
    #C3
    if int(array[C3]) >= 27:
        result[C3] = "High"
    elif int(array[C3]) >= 25:
        result[C3] = "Medium_High"
    elif int(array[C3]) >= 23:
        result[C3] = "Medium_Low"
    else:
        result[C3] = "Low"
    #C4
    if int(array[C4]) >= 23:
        result[C4] = "High"
    elif int(array[C4]) >= 21:
        result[C4] = "Medium_High"
    elif int(array[C4]) >= 19:
        result[C4] = "Medium_Low"
    else:
        result[C4] = "Low"
    #C5
    if int(array[C5]) >= 23:
        result[C5] = "High"
    elif int(array[C5]) >= 20:
        result[C5] = "Medium_High"
    elif int(array[C5]) >= 18:
        result[C5] = "Medium_Low"
    else:
        result[C5] = "Low"
    #C6
    if int(array[C6]) >= 24:
        result[C6] = "High"
    elif int(array[C6]) >= 23:
        result[C6] = "Medium_High"
    elif int(array[C6]) >= 22:
        result[C6] = "Medium_Low"
    else:
        result[C6] = "Low"
    #Internal
    if int(array[Int]) >= 32:
        result[Int] = "High"
    elif int(array[Int]) >= 24:
        result[Int] = "Medium_High"
    elif int(array[Int]) >= 12:
        result[Int] = "Medium_Low"
    else:
        result[Int] = "Low"
    #Powerful Others
    if int(array[Pow]) >= 32:
        result[Pow] = "High"
    elif int(array[Pow]) >= 24:
        result[Pow] = "Medium_High"
    elif int(array[Pow]) >= 12:
        result[Pow] = "Medium_Low"
    else:
        result[Pow] = "Low"
    #Chance
    if int(array[Cha]) >= 32:
        result[Cha] = "High"
    elif int(array[Cha]) >= 24:
        result[Cha] = "Medium_High"
    elif int(array[Cha]) >= 12:
        result[Cha] = "Medium_Low"
    else:
        result[Cha] = "Low"
    return result
