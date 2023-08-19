import axios from "axios";
import { toast } from "react-toastify";
import { Role, User } from "./model/user";

interface Location {
  latitude: number;
  longitude: number;
}

export const getStepsBetweenMapPoints = async ({ source, destination }: { source: Location, destination: Location }) => {
  const steps: any[] = [];
  try {
    const res = await axios.get(
      `https://router.project-osrm.org/route/v1/car/${source.longitude},${source.latitude};${destination.longitude},${destination.latitude}?steps=true`
    );
    res.data.routes[0].legs[0].steps.forEach((step: any) => {
      const locs = step.intersections.map((i: any) => ({
        latitude: i.location[1],
        longitude: i.location[0],
      }));
      locs.forEach((loc: any) => {
        steps.push([loc.latitude, loc.longitude]);
      });
    });
    console.log(steps.toString());
  } catch (e) { }
  return steps
}

export const extractErrorMessages = (error: any): string[] => {
  const e = error.response.data
  if (e.message) {
    return e.message.split(";")
  }
  return error.message ? [error.message] : []
}

export const notifyErrors = (errors: string[]) => {
  errors.forEach(e => toast.error(e))
}

export const getActiveRole = (userData: User | null): Role | undefined => {
  if (userData && userData.roles) {
    const adminRole = userData.roles.find(role => role.name === "Admin")
    if (adminRole) {
      return adminRole
    }
    return userData.roles.find(role => role.name === "User")
  }
}

export const convertDate = (dateStr: string): Date => {
  let date = new Date(dateStr);
  date = new Date(date.setHours(date.getHours() - date.getTimezoneOffset() / 60))
  return date;
}

export const deepEqual = (obj1: any, obj2: any) => {
  return JSON.stringify(obj1) === JSON.stringify(obj2)
}

export const deepCopy = (obj: any) => {
  return JSON.parse(JSON.stringify(obj))
}