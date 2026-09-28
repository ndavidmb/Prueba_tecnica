import * as yup from 'yup';

export interface LeaderboardEntry {
  userId: number;
  userName: string;
  totalPoints: number;
  totalBets: number;
  rankPosition: number;
}

export interface UserBetHistoryItem {
  betId: number;
  matchId: number;
  localTeamName: string;
  visitorTeamName: string;
  predictedLocalGoals: number;
  predictedVisitorGoals: number;
  realLocalGoals: number | null;
  realVisitorGoals: number | null;
  pointsEarned: number;
}

export interface UserHistory {
  userId: number;
  userName: string;
  totalPoints: number;
  bets: UserBetHistoryItem[];
}

export const leaderboardEntrySchema: yup.ObjectSchema<LeaderboardEntry> = yup.object({
  userId: yup.number().required(),
  userName: yup.string().required(),
  totalPoints: yup.number().required(),
  totalBets: yup.number().required(),
  rankPosition: yup.number().required(),
});

export const leaderboardListSchema = yup.array().of(leaderboardEntrySchema).required();

const userBetHistoryItemSchema: yup.ObjectSchema<UserBetHistoryItem> = yup.object({
  betId: yup.number().required(),
  matchId: yup.number().required(),
  localTeamName: yup.string().required(),
  visitorTeamName: yup.string().required(),
  predictedLocalGoals: yup.number().required(),
  predictedVisitorGoals: yup.number().required(),
  realLocalGoals: yup.number().nullable().defined(),
  realVisitorGoals: yup.number().nullable().defined(),
  pointsEarned: yup.number().required(),
});

export const userHistorySchema: yup.ObjectSchema<UserHistory> = yup.object({
  userId: yup.number().required(),
  userName: yup.string().required(),
  totalPoints: yup.number().required(),
  bets: yup.array().of(userBetHistoryItemSchema).required(),
});
