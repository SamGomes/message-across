#!/usr/bin/python
import sys
import os
import pandas as pd
from compileTwoParts import processTwoParts
from compileThreeParts import processThreeParts
from compileFourParts import processFourParts

twoParts = open("./output/compiledUsersTwoParts.csv", 'w')
threeParts = open("./output/compiledUsersThreeParts.csv", 'w')
fourParts = open("./output/compiledUsersFourParts.csv", 'w')

fname = "./input/messageAcrossData.csv"

data_df = pd.read_csv('./input/messageAcrossData.csv', delimiter=',')

#print(data_df.columns)

personality_df = data_df[['playerId',   'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
                                        'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
                                        'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
                                        'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
                                        'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
                                        'N', 'E', 'O', 'A', 'C',
                                        'Internal', 'PowerfulOthers', 'Chance',
                                        'gender', 'age']]
#print(personality_df.head())

with open('./input/messageAcrossData.csv','r') as f:
    headerVars = ['playerId',           'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
                                        'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
                                        'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
                                        'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
                                        'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
                                        'N', 'E', 'O', 'A', 'C',
                                        'Internal', 'PowerfulOthers', 'Chance']
    headerFinal = ','.join(headerVars) + ','
    # print(headerFinal)
    
    twoParts.write(headerFinal + '\n')
    threeParts.write(headerFinal + '\n')
    fourParts.write(headerFinal + '\n')

for index, row in personality_df.iterrows():
    #print(row.to_numpy())
    for value in processTwoParts(row.to_numpy()):
        twoParts.write(str(value) + ',')
    twoParts.write('\n')
    for value in processThreeParts(row.to_numpy()):
        threeParts.write(str(value) + ',')
    threeParts.write('\n')
    for value in processFourParts(row.to_numpy()):
        fourParts.write(str(value) + ',')
    fourParts.write('\n')

twoParts.close()
threeParts.close()
fourParts.close()
