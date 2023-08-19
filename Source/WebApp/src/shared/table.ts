import { twMerge } from "tailwind-merge";

const tableStyles = "grid gap-4 grid-cols-8 border-light-3 py-2 px-4 border-b-[1px] border-x-[1px] items-center auto-cols-fr text-dark-1";

export const tableHeaderStyles = twMerge(
  tableStyles,
  "border-t-[1px] rounded-tr-lg rounded-tl-lg text-left font-bold sticky top-[-5px] pt-3 left-0 bg-light-2"
)

export const tableContentStyles = twMerge(
  tableStyles,
  "text-sm last:rounded-br-lg last:rounded-bl-lg"
)