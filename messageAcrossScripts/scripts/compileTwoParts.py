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

def processTwoParts(array):
    #print(array)
    result = list(array)
    #N
    if int(array[N]) >= 86:
        result[N] = "High"
    else:
        result[N] = "Low"
    #N1
    if int(array[N1]) >= 19:
        result[N1] = "High"
    else:
        result[N1] = "Low"
    #N2
    if int(array[N2]) >= 15:
        result[N2] = "High"
    else:
        result[N2] = "Low"
    #N3
    if int(array[N3]) >= 16:
        result[N3] = "High"
    else:
        result[N3] = "Low"
    #N4
    if int(array[N4]) >= 15:
        result[N4] = "High"
    else:
        result[N4] = "Low"
    #N5
    if int(array[N5]) >= 15:
        result[N5] = "High"
    else:
        result[N5] = "Low"
    #N6
    if int(array[N6]) >= 12:
        result[N6] = "High"
    else:
        result[N6] = "Low"
    #E
    if int(array[E]) >= 102:
        result[E] = "High"
    else:
        result[E] = "Low"
    #E1
    if int(array[E1]) >= 22:
        result[E1] = "High"
    else:
        result[E1] = "Low"
    #E2
    if int(array[E2]) >= 17:
        result[E2] = "High"
    else:
        result[E2] = "Low"
    #E3
    if int(array[E3]) >= 15:
        result[E3] = "High"
    else:
        result[E3] = "Low"
    #E4
    if int(array[E4]) >= 17:
        result[E4] = "High"
    else:
        result[E4] = "Low"
    #E5
    if int(array[E5]) >= 18:
        result[E5] = "High"
    else:
        result[E5] = "Low"
    #E6
    if int(array[E6]) >= 19:
        result[E6] = "High"
    else:
        result[E6] = "Low"
    #O
    if int(array[O]) >= 100:
        result[O] = "High"
    else:
        result[O] = "Low"
    #O1
    if int(array[O1]) >= 17:
        result[O1] = "High"
    else:
        result[O1] = "Low"
    #O2
    if int(array[O2]) >= 20:
        result[O2] = "High"
    else:
        result[O2] = "Low"
    #O3
    if int(array[O3]) >= 20:
        result[O3] = "High"
    else:
        result[O3] = "Low"
    #O4
    if int(array[O4]) >= 17:
        result[O4] = "High"
    else:
        result[O4] = "Low"
    #O5
    if int(array[O5]) >= 16:
        result[O5] = "High"
    else:
        result[O5] = "Low"
    #O6
    if int(array[O6]) >= 17:
        result[O6] = "High"
    else:
        result[O6] = "Low"
    #A
    if int(array[A]) >= 123:
        result[A] = "High"
    else:
        result[A] = "Low"
    #A1
    if int(array[A1]) >= 19:
        result[A1] = "High"
    else:
        result[A1] = "Low"
    #A2
    if int(array[A2]) >= 20:
        result[A2] = "High"
    else:
        result[A2] = "Low"
    #A3
    if int(array[A3]) >= 23:
        result[A3] = "High"
    else:
        result[A3] = "Low"
    #A4
    if int(array[A4]) >= 21:
        result[A4] = "High"
    else:
        result[A4] = "Low"
    #A5
    if int(array[A5]) >= 21:
        result[A5] = "High"
    else:
        result[A5] = "Low"
    #A6
    if int(array[A6]) >= 22:
        result[A6] = "High"
    else:
        result[A6] = "Low"
    #C
    if int(array[C]) >= 129:
        result[C] = "High"
    else:
        result[C] = "Low"
    #C1
    if int(array[C1]) >= 21:
        result[C1] = "High"
    else:
        result[C1] = "Low"
    #C2
    if int(array[C2]) >= 21:
        result[C2] = "High"
    else:
        result[C2] = "Low"
    #C3
    if int(array[C3]) >= 25:
        result[C3] = "High"
    else:
        result[C3] = "Low"
    #C4
    if int(array[C4]) >= 21:
        result[C4] = "High"
    else:
        result[C4] = "Low"
    #C5
    if int(array[C5]) >= 20:
        result[C5] = "High"
    else:
        result[C5] = "Low"
    #C6
    if int(array[C6]) >= 23:
        result[C6] = "High"
    else:
        result[C6] = "Low"
    #Internal
    if int(array[Int]) >= 24:
        result[Int] = "High"
    else:
        result[Int] = "Low"
    #Powerful Others
    if int(array[Pow]) >= 24:
        result[Pow] = "High"
    else:
        result[Pow] = "Low"
    #Chance
    if int(array[Cha]) >= 24:
        result[Cha] = "High"
    else:
        result[Cha] = "Low"
    return result
