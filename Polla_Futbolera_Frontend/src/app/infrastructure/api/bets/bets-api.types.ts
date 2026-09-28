import * as yup from 'yup';

export interface CreateBetRequest {
  matchId: number;
  localGoals: number;
  visitorGoals: number;
}

export interface BetResult {
  betId: number;
  userId: number;
  matchId: number;
  predictedLocalGoals: number;
  predictedVisitorGoals: number;
  realLocalGoals: number | null;
  realVisitorGoals: number | null;
  pointsEarned: number;
  isExactMatch: boolean;
  isTrendMatch: boolean;
}

export const betResultSchema: yup.ObjectSchema<BetResult> = yup.object({
  betId: yup.number().required(),
  userId: yup.number().required(),
  matchId: yup.number().required(),
  predictedLocalGoals: yup.number().required(),
  predictedVisitorGoals: yup.number().required(),
  realLocalGoals: yup.number().nullable().defined(),
  realVisitorGoals: yup.number().nullable().defined(),
  pointsEarned: yup.number().required(),
  isExactMatch: yup.boolean().required(),
  isTrendMatch: yup.boolean().required(),
});
