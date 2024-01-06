suppressMessages(library(ggplot2))
suppressMessages(library(multcomp))
suppressMessages(library(nlme))
suppressMessages(library(pastecs))
suppressMessages(library(reshape))
suppressMessages(library(tidyverse))
suppressMessages(library(sjPlot))
suppressMessages(library(sjmisc))
suppressMessages(library(dplyr))
suppressMessages(library(emmeans))

oldw <- getOption("warn")
options(warn = -1)

print("Plotting main effects in facets...")

myData <- read.csv(file="output/meltedDataThreeCategories.csv", header=TRUE, sep=",")

myData<-myData[!(myData$ScoreSystem=="B" | myData$ScoreSystem=="D"),]

levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "A"] <- "Competition"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "C"] <- "Mutual Help"

#myData$ScoreSystem <- factor(myData$ScoreSystem, levels=rev(levels(myData$ScoreSystem)))

myData$C <- factor(myData$C , levels=c("High", "Medium", "Low"))
myData$A <- factor(myData$A , levels=c("High", "Medium", "Low"))

lm0 = lm(takes ~ C * ScoreSystem, data = myData)
x <- emmip(lm0, C ~ ScoreSystem, engine="ggplot", CIs = TRUE) + labs(x="Game Version", y="Average Take Actions", colour="Conscienciousness") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(0,4)) + scale_x_discrete(labels=c("Competition", "Mutual Help"))
x + scale_colour_manual(values=c("#d7301f", "#fc8d59", "#fdcc8a"), labels=c("High", "Medium", "Low"), guide = guide_legend(reverse = FALSE)) #+ geom_jitter(aes(x = ScoreSystem, y = takes, colour = C), data = myData, pch = 20, width = 0.1, size=2) 

suppressMessages(ggsave("plots/interactionEffects/takes/takes.png", height = 4, width = 10))

lm0 = lm(who ~ A * ScoreSystem, data = myData)
x <- emmip(lm0, A ~ ScoreSystem, engine="ggplot", CIs = TRUE) + labs(x="Game Version", y="Focus", colour="Agreeableness") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(1,7)) + scale_x_discrete(labels=c("Competition", "Mutual Help"))
x + scale_colour_manual(values=c("#d7301f", "#fc8d59", "#fdcc8a"), labels=c("High", "Medium", "Low"), guide = guide_legend(reverse = FALSE)) + scale_y_reverse("Focus", labels = as.character(c("Me  3","2", "1", "Neutral  0", "-1", "-2", "The Other  -3")), breaks = c(1,2,3,4,5,6,7)) #+ geom_jitter(aes(x = ScoreSystem, y = takes, colour = C), data = myData, pch = 20, width = 0.1, size=2) 

suppressMessages(ggsave("plots/interactionEffects/who/who.png", height = 4, width = 10))

lm0 = lm(what ~ A * ScoreSystem, data = myData)
x <- emmip(lm0, A ~ ScoreSystem, engine="ggplot", CIs = TRUE) + labs(x="Game Version", y="Social Valence", colour="Agreeableness") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(1,7)) + scale_x_discrete(labels=c("Competition", "Mutual Help"))
x + scale_colour_manual(values=c("#d7301f", "#fc8d59", "#fdcc8a"), labels=c("High", "Medium", "Low"), guide = guide_legend(reverse = FALSE)) + scale_y_reverse("Social Valence", labels = as.character(c("Help  3","2", "1", "Neutral  0", "-1", "-2", "Complicate  -3")), breaks = c(1,2,3,4,5,6,7)) #+ geom_jitter(aes(x = ScoreSystem, y = takes, colour = C), data = myData, pch = 20, width = 0.1, size=2) 

suppressMessages(ggsave("plots/interactionEffects/what/what.png", height = 4, width = 10))

options(warn = oldw)

