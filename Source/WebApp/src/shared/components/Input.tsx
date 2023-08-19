import { forwardRef } from "react";
import { twMerge } from "tailwind-merge";

export const Input = forwardRef((props: any, ref) => {
  const { id, label, variant, className, placeholder, ...remainingProps } = props;
  const classes =
    "mt-1 bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500";
  return (
    <div>
      <label htmlFor={id}>{label}</label>
      <input
        ref={ref}
        type="text"
        id={id}
        className={twMerge(classes, className)}
        placeholder={placeholder}
        {...remainingProps}
      />
    </div>
  );
})