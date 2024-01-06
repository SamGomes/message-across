import pandas as pd 

data = pd.read_csv("input/messageAcrossData.csv")

dataTwo = pd.read_csv("output/compiledUsersTwoParts.csv") 
dataThree = pd.read_csv("output/compiledUsersThreeParts.csv") 
dataFour = pd.read_csv("output/compiledUsersFourParts.csv") 

other_df = data.drop([	'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
						'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
						'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
						'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
						'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
						'N', 'E', 'O', 'A', 'C',
						'Internal', 'PowerfulOthers', 'Chance'], axis=1)

#############################################################
#############################################################
#############################################################
#############################################################	TWO GROUPS
#############################################################
#############################################################
#############################################################

mergedTwo_df = dataTwo.merge(other_df, on='playerId')
mergedTwo_df = mergedTwo_df.loc[:, ~mergedTwo_df.columns.str.contains('^Unnamed')]
mergedTwo_df.to_csv('input/dataTwoCategories.csv', index=False, header=True)

#################################### Gives

gives_df = mergedTwo_df[[		'playerId', 'meanNumberOfGives_A', 'meanNumberOfGives_B', 'meanNumberOfGives_C', 'meanNumberOfGives_D']]

gives_df.rename(columns={'meanNumberOfGives_A':'A', 'meanNumberOfGives_B':'B', 'meanNumberOfGives_C':'C', 'meanNumberOfGives_D':'D'}, inplace=True)

gives_melted_df = pd.melt(gives_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

gives_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'gives'}, inplace=True)

#################################### Takes

takes_df = mergedTwo_df[[		'playerId', 'meanNumberOfTakes_A', 'meanNumberOfTakes_B', 'meanNumberOfTakes_C', 'meanNumberOfTakes_D']]

takes_df.rename(columns={'meanNumberOfTakes_A':'A', 'meanNumberOfTakes_B':'B', 'meanNumberOfTakes_C':'C', 'meanNumberOfTakes_D':'D'}, inplace=True)

takes_melted_df = pd.melt(takes_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

takes_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'takes'}, inplace=True)

#################################### Who

who_df = mergedTwo_df[[		'playerId', 'whoFocus_A', 'whoFocus_B', 'whoFocus_C', 'whoFocus_D']]

who_df.rename(columns={'whoFocus_A':'A', 'whoFocus_B':'B', 'whoFocus_C':'C', 'whoFocus_D':'D'}, inplace=True)

who_melted_df = pd.melt(who_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

who_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'who'}, inplace=True)

#################################### What

what_df = mergedTwo_df[[		'playerId', 'whatFocus_A', 'whatFocus_B', 'whatFocus_C', 'whatFocus_D']]

what_df.rename(columns={'whatFocus_A':'A', 'whatFocus_B':'B', 'whatFocus_C':'C', 'whatFocus_D':'D'}, inplace=True)

what_melted_df = pd.melt(what_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

what_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'what'}, inplace=True)

#################################### Final Merge Two

personality_df = mergedTwo_df[[	'playerId', 'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
									'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
									'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
									'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
									'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
									'N', 'E', 'O', 'A', 'C',
									'Internal', 'PowerfulOthers', 'Chance']]

merged_df = personality_df.merge(gives_melted_df, on='playerId')
merged_df = merged_df.merge(takes_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(who_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(what_melted_df, on=['playerId', 'ScoreSystem'])

merged_df.to_csv('output/meltedDataTwoCategories.csv', index=False, header=True)

#############################################################
#############################################################
#############################################################
#############################################################	THREE GROUPS
#############################################################
#############################################################
#############################################################

mergedThree_df = dataThree.merge(other_df, on='playerId')
mergedThree_df = mergedThree_df.loc[:, ~mergedThree_df.columns.str.contains('^Unnamed')]
mergedThree_df.to_csv('input/dataThreeCategories.csv', index=False, header=True)

#################################### Gives

gives_df = mergedThree_df[[		'playerId', 'meanNumberOfGives_A', 'meanNumberOfGives_B', 'meanNumberOfGives_C', 'meanNumberOfGives_D']]

gives_df.rename(columns={'meanNumberOfGives_A':'A', 'meanNumberOfGives_B':'B', 'meanNumberOfGives_C':'C', 'meanNumberOfGives_D':'D'}, inplace=True)

gives_melted_df = pd.melt(gives_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

gives_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'gives'}, inplace=True)

#################################### Takes

takes_df = mergedThree_df[[		'playerId', 'meanNumberOfTakes_A', 'meanNumberOfTakes_B', 'meanNumberOfTakes_C', 'meanNumberOfTakes_D']]

takes_df.rename(columns={'meanNumberOfTakes_A':'A', 'meanNumberOfTakes_B':'B', 'meanNumberOfTakes_C':'C', 'meanNumberOfTakes_D':'D'}, inplace=True)

takes_melted_df = pd.melt(takes_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

takes_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'takes'}, inplace=True)

#################################### Who

who_df = mergedThree_df[[		'playerId', 'whoFocus_A', 'whoFocus_B', 'whoFocus_C', 'whoFocus_D']]

who_df.rename(columns={'whoFocus_A':'A', 'whoFocus_B':'B', 'whoFocus_C':'C', 'whoFocus_D':'D'}, inplace=True)

who_melted_df = pd.melt(who_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

who_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'who'}, inplace=True)

#################################### What

what_df = mergedThree_df[[		'playerId', 'whatFocus_A', 'whatFocus_B', 'whatFocus_C', 'whatFocus_D']]

what_df.rename(columns={'whatFocus_A':'A', 'whatFocus_B':'B', 'whatFocus_C':'C', 'whatFocus_D':'D'}, inplace=True)

what_melted_df = pd.melt(what_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

what_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'what'}, inplace=True)

#################################### Final Merge Two

personality_df = mergedThree_df[[	'playerId', 'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
									'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
									'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
									'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
									'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
									'N', 'E', 'O', 'A', 'C',
									'Internal', 'PowerfulOthers', 'Chance']]

merged_df = personality_df.merge(gives_melted_df, on='playerId')
merged_df = merged_df.merge(takes_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(who_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(what_melted_df, on=['playerId', 'ScoreSystem'])

merged_df.to_csv('output/meltedDataThreeCategories.csv', index=False, header=True)

#############################################################
#############################################################
#############################################################
#############################################################	FOUR GROUPS
#############################################################
#############################################################
#############################################################

mergedFour_df = dataFour.merge(other_df, on='playerId')
mergedFour_df = mergedFour_df.loc[:, ~mergedFour_df.columns.str.contains('^Unnamed')]
mergedFour_df.to_csv('input/dataFourCategories.csv', index=False, header=True)

#################################### Gives

gives_df = mergedFour_df[[		'playerId', 'meanNumberOfGives_A', 'meanNumberOfGives_B', 'meanNumberOfGives_C', 'meanNumberOfGives_D']]

gives_df.rename(columns={'meanNumberOfGives_A':'A', 'meanNumberOfGives_B':'B', 'meanNumberOfGives_C':'C', 'meanNumberOfGives_D':'D'}, inplace=True)

gives_melted_df = pd.melt(gives_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

gives_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'gives'}, inplace=True)

#################################### Takes

takes_df = mergedFour_df[[		'playerId', 'meanNumberOfTakes_A', 'meanNumberOfTakes_B', 'meanNumberOfTakes_C', 'meanNumberOfTakes_D']]

takes_df.rename(columns={'meanNumberOfTakes_A':'A', 'meanNumberOfTakes_B':'B', 'meanNumberOfTakes_C':'C', 'meanNumberOfTakes_D':'D'}, inplace=True)

takes_melted_df = pd.melt(takes_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

takes_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'takes'}, inplace=True)

#################################### Who

who_df = mergedFour_df[[		'playerId', 'whoFocus_A', 'whoFocus_B', 'whoFocus_C', 'whoFocus_D']]

who_df.rename(columns={'whoFocus_A':'A', 'whoFocus_B':'B', 'whoFocus_C':'C', 'whoFocus_D':'D'}, inplace=True)

who_melted_df = pd.melt(who_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

who_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'who'}, inplace=True)

#################################### What

what_df = mergedFour_df[[		'playerId', 'whatFocus_A', 'whatFocus_B', 'whatFocus_C', 'whatFocus_D']]

what_df.rename(columns={'whatFocus_A':'A', 'whatFocus_B':'B', 'whatFocus_C':'C', 'whatFocus_D':'D'}, inplace=True)

what_melted_df = pd.melt(what_df, id_vars =['playerId'], value_vars =['A', 'B', 'C', 'D'])

what_melted_df.rename(columns={'variable':'ScoreSystem', 'value':'what'}, inplace=True)

#################################### Final Merge Two

personality_df = mergedFour_df[[	'playerId', 'N1', 'N2', 'N3', 'N4', 'N5', 'N6',
									'E1', 'E2', 'E3', 'E4', 'E5', 'E6',
									'O1', 'O2', 'O3', 'O4', 'O5', 'O6',
									'A1', 'A2', 'A3', 'A4', 'A5', 'A6',
									'C1', 'C2', 'C3', 'C4', 'C5', 'C6',
									'N', 'E', 'O', 'A', 'C',
									'Internal', 'PowerfulOthers', 'Chance']]

merged_df = personality_df.merge(gives_melted_df, on='playerId')
merged_df = merged_df.merge(takes_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(who_melted_df, on=['playerId', 'ScoreSystem'])
merged_df = merged_df.merge(what_melted_df, on=['playerId', 'ScoreSystem'])

merged_df.to_csv('output/meltedDataFourCategories.csv', index=False, header=True)