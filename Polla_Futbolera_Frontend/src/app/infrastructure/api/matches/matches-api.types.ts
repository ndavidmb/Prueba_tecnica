import * as yup from 'yup';

export enum MatchStatus {
  UpcomingMatch = 1,
  FullTime = 2,
}

export interface Team {
  id: number;
  name: string;
}

export interface Match {
  id: number;
  localTeam: Team;
  visitorTeam: Team;
  localGoals: number | null;
  visitorGoals: number | null;
  status: MatchStatus;
}

export interface MatchResultUpdate {
  id: number;
  localGoals: number | null;
  visitorGoals: number | null;
  status: MatchStatus;
}

export interface UpdateMatchResultRequest {
  localGoals: number;
  visitorGoals: number;
}

const teamSchema: yup.ObjectSchema<Team> = yup.object({
  id: yup.number().required(),
  name: yup.string().required(),
});

const statusSchema = yup
  .mixed<MatchStatus>()
  .oneOf([MatchStatus.UpcomingMatch, MatchStatus.FullTime])
  .required();

export const matchSchema: yup.ObjectSchema<Match> = yup.object({
  id: yup.number().required(),
  localTeam: teamSchema.required(),
  visitorTeam: teamSchema.required(),
  localGoals: yup.number().nullable().defined(),
  visitorGoals: yup.number().nullable().defined(),
  status: statusSchema,
});

export const matchListSchema = yup.array().of(matchSchema).required();

export const matchResultUpdateSchema: yup.ObjectSchema<MatchResultUpdate> = yup.object({
  id: yup.number().required(),
  localGoals: yup.number().nullable().defined(),
  visitorGoals: yup.number().nullable().defined(),
  status: statusSchema,
});
