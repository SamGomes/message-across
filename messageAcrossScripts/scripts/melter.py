import pandas as pd 

data = pd.read_csv("input/messageAcrossData.csv") 

#print(data.head())

personality_df = data[[	'playerId', 'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
									'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
									'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
									'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
									'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
									'N', 'E', 'O', 'A', 'C',
									'Internal', 'PowerfulOthers', 'Chance']]

#print(personality_df.head())

#################################### Gives

gives_df = data[[		'playerId', 'meanNumberOfGives_A', 'meanNumberOfGives_B', 'meanNumberOfGives_C', 'meanNumberOfGives_D']]

gives_df.rename(columns={'meanNumberOfGives_A':'A', 'meanNumberOfGives_B':'B', 'meanNumberOfGives_C':'C', 'meanNumberOfGives_D':'D'}, inplace=True)

gives_melted_df = pd.melt(gives_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

gives_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'gives'}, inplace=True)

#print(gives_melted_df)

#################################### Takes

takes_df = data[[		'playerId', 'meanNumberOfTakes_A', 'meanNumberOfTakes_B', 'meanNumberOfTakes_C', 'meanNumberOfTakes_D']]

takes_df.rename(columns={'meanNumberOfTakes_A':'A', 'meanNumberOfTakes_B':'B', 'meanNumberOfTakes_C':'C', 'meanNumberOfTakes_D':'D'}, inplace=True)

takes_melted_df = pd.melt(takes_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

takes_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'takes'}, inplace=True)

#print(takes_melted_df)

#################################### Who

who_df = data[[		'playerId', 'whoFocus_A', 'whoFocus_B', 'whoFocus_C', 'whoFocus_D']]

who_df.rename(columns={'whoFocus_A':'A', 'whoFocus_B':'B', 'whoFocus_C':'C', 'whoFocus_D':'D'}, inplace=True)

who_melted_df = pd.melt(who_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

who_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'who'}, inplace=True)

#print(who_melted_df)

#################################### What

what_df = data[[		'playerId', 'whatFocus_A', 'whatFocus_B', 'whatFocus_C', 'whatFocus_D']]

what_df.rename(columns={'whatFocus_A':'A', 'whatFocus_B':'B', 'whatFocus_C':'C', 'whatFocus_D':'D'}, inplace=True)

what_melted_df = pd.melt(what_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

what_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'what'}, inplace=True)

#print(what_melted_df)

merged_df = personality_df.merge(gives_melted_df, on='playerId')
merged_df = merged_df.merge(takes_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(who_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(what_melted_df, on=['playerId', 'ScoreSystem'])

#print(merged_df)

merged_df.to_csv('output/meltedData.csv', index=False, header=True)