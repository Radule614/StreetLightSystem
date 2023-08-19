import { Team } from "./team";

export interface Repair {
  id: string;
  startDate: string;
  endDate: string;
  teamId: string;
  team?: Team;
  poleId: string;
  isSuccessful: boolean;
  isFinished: boolean
}