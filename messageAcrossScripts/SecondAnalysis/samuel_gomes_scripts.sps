* Encoding: UTF-8.
*-----------------------------Takes-------------------------------.

NPAR TESTS 
  /FRIEDMAN=meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /MISSING LISTWISE 
  /METHOD=EXACT TIMER(5).

GLM meanTakes_A_Ranked meanTakes_B_Ranked meanTakes_C_Ranked meanTakes_D_Ranked WITH RN RE RO RA RC 
  /WSFACTOR=version 4 Polynomial 
  /MEASURE=meanNumberOfTakes 
  /METHOD=SSTYPE(3) 
  /EMMEANS=TABLES(OVERALL) WITH(RN=MEAN RC=MEAN RE=MEAN RA=MEAN RO=MEAN) 
  /EMMEANS=TABLES(version) WITH(RN=MEAN RC=MEAN RE=MEAN RA=MEAN RO=MEAN) COMPARE ADJ(BONFERRONI) 
  /PRINT=DESCRIPTIVE ETASQ OPOWER HOMOGENEITY 
  /CRITERIA=ALPHA(.05) 
  /WSDESIGN=version 
  /DESIGN=RN RC RE RA RO.

NONPAR CORR 
  /VARIABLES=N E O A C meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.

NONPAR CORR 
  /VARIABLES=N1 N2 N3 N4 N5 N6 meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=E1 E2 E3 E4 E5 E6 meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=O1 O2 O3 O4 O5 O6 meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=A1 A2 A3 A4 A5 A6 meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=C1 C2 C3 C4 C5 C6 meanNumberOfTakes_A meanNumberOfTakes_B meanNumberOfTakes_C meanNumberOfTakes_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  


*-----------------------------Focus-------------------------------.

NPAR TESTS 
  /FRIEDMAN=whoFocus_A whoFocus_B whoFocus_C  whoFocus_D 
  /MISSING LISTWISE 
  /METHOD=EXACT TIMER(5).

GLM focus_A_Ranked focus_B_Ranked focus_C_Ranked focus_D_Ranked WITH RN RE RO RA RC 
  /WSFACTOR=version 4 Polynomial 
  /MEASURE=focus 
  /METHOD=SSTYPE(3) 
  /EMMEANS=TABLES(OVERALL) WITH(RN=MEAN RC=MEAN RE=MEAN RA=MEAN RO=MEAN) 
  /EMMEANS=TABLES(version) WITH(RN=MEAN RC=MEAN RE=MEAN RA=MEAN RO=MEAN) COMPARE ADJ(BONFERRONI) 
  /PRINT=DESCRIPTIVE ETASQ OPOWER HOMOGENEITY 
  /CRITERIA=ALPHA(.05) 
  /WSDESIGN=version 
  /DESIGN=RN RE RO RA RC.

NONPAR CORR 
  /VARIABLES=N E O A C whoFocus_A whoFocus_B whoFocus_C whoFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.


NONPAR CORR 
  /VARIABLES=N1 N2 N3 N4 N5 N6 whoFocus_A whoFocus_B whoFocus_C whoFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=E1 E2 E3 E4 E5 E6 whoFocus_A whoFocus_B whoFocus_C whoFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=O1 O2 O3 O4 O5 O6 whoFocus_A whoFocus_B whoFocus_C whoFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=A1 A2 A3 A4 A5 A6 whoFocus_A whoFocus_B whoFocus_C whoFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=C1 C2 C3 C4 C5 C6 whoFocus_A whoFocus_B whoFocus_C whoFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.


*-----------------------------Challenge-------------------------------.

NPAR TESTS 
  /FRIEDMAN=whatFocus_A whatFocus_B whatFocus_C  whatFocus_D 
  /MISSING LISTWISE 
  /METHOD=EXACT TIMER(30).

GLM challenge_A_Ranked challenge_B_Ranked challenge_C_Ranked challenge_D_Ranked WITH RN RE RO RA RC
  /WSFACTOR=version 4 Polynomial 
  /MEASURE=challenge 
  /METHOD=SSTYPE(3) 
  /EMMEANS=TABLES(OVERALL) WITH(RN=MEAN RC=MEAN RE=MEAN RA=MEAN RO=MEAN) 
  /EMMEANS=TABLES(version) WITH(RN=MEAN RC=MEAN RE=MEAN RA=MEAN RO=MEAN) COMPARE ADJ(BONFERRONI) 
  /PRINT=DESCRIPTIVE ETASQ OPOWER HOMOGENEITY 
  /CRITERIA=ALPHA(.05) 
  /WSDESIGN=version 
  /DESIGN=RN RE RO RA RC.


NONPAR CORR 
  /VARIABLES=N E O A C whatFocus_A whatFocus_B whatFocus_C whatFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.



NONPAR CORR 
  /VARIABLES=N1 N2 N3 N4 N5 N6 whatFocus_A whatFocus_B whatFocus_C whatFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=E1 E2 E3 E4 E5 E6 whatFocus_A whatFocus_B whatFocus_C whatFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=O1 O2 O3 O4 O5 O6 whatFocus_A whatFocus_B whatFocus_C whatFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=A1 A2 A3 A4 A5 A6 whatFocus_A whatFocus_B whatFocus_C whatFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
  
NONPAR CORR 
  /VARIABLES=C1 C2 C3 C4 C5 C6 whatFocus_A whatFocus_B whatFocus_C whatFocus_D 
  /PRINT=SPEARMAN TWOTAIL NOSIG 
  /MISSING=PAIRWISE.
