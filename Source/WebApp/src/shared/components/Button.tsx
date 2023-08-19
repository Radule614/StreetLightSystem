import { forwardRef } from "react";
import { twMerge } from "tailwind-merge";

const variants = {
  default:
    "text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 dark:bg-blue-600 dark:hover:bg-blue-700 focus:outline-none dark:focus:ring-blue-800",
  alternative:
    "py-2.5 px-5 text-sm font-medium text-gray-900 focus:outline-none bg-white rounded-lg border border-gray-200 hover:bg-gray-100 hover:text-blue-700 focus:z-10 focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:border-gray-600 dark:hover:text-white dark:hover:bg-gray-700",
  error:
    "focus:outline-none text-white bg-red-700 hover:bg-red-800 focus:ring-4 focus:ring-red-300 font-medium rounded-lg text-sm px-5 py-2.5 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-900",
  warn: "focus:outline-none text-white bg-yellow-400 hover:bg-yellow-500 focus:ring-4 focus:ring-yellow-300 font-medium rounded-lg text-sm px-5 py-2.5 dark:focus:ring-yellow-900",
};

export const Button = forwardRef((props: any, ref) => {
  const { variant, children, onClick, className, type, ...remainingProps } =
    props;
  const classes: string =
    variants[
      variant
        ? (variant as "default" | "alternative" | "error" | "warn")
        : "default"
    ];

  const buttonType = type ?? "button";
  return (
    <button
      ref={ref}
      onClick={onClick}
      className={twMerge(classes, className)}
      type={buttonType}
      {...remainingProps}
    >
      {children}
    </button>
  );
});
