export interface Pole {
  id: string;
  status: number;
  latitude: number;
  longitude: number;
}

export const statusToString = (status: number) => {
  switch(status){
    case 1: return "Broken"
    case 2: return "Being Repaired"
  }
  return "Working"
}

export const statusToColor = (status: number) => {
  switch (status) {
    case 1:
      return "text-red-600";
    case 2:
      return "text-yellow-400";
  }
  return "text-green-600";
}