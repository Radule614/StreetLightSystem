export interface User {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  roles?: Role[];
}

export interface Role {
  name: string;
}