export interface Team {
  id?: string;
  name: string;
  members?: Member[];
}

export interface Member {
  id: string;
  firstName: string;
  lastName: string;
  team?: Team;
}