import { useEffect, useState } from "react";

export const Checkbox = (props: any) => {
  const { id, label, isChecked, valueChanged, className, ...remainingProps } =
    props;
  const [_isChecked, _setIsChecked] = useState(false);
  useEffect(() => {
    if (isChecked !== undefined) {
      _setIsChecked(isChecked);
    }
  }, [isChecked]);

  const handleChange = (event: any) => {
    _setIsChecked(event.target.checked);
    if (valueChanged) {
      valueChanged(event.target.checked);
    }
  };

  return (
    <div className="flex items-center">
      <input
        id={id}
        type="checkbox"
        value=""
        className="cursor-pointer w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 dark:focus:ring-blue-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600"
        onChange={handleChange}
        checked={_isChecked}
        {...remainingProps}
      />
      {label && (
        <label
          htmlFor={id}
          className="text-sm font-medium text-gray-900 dark:text-gray-300"
        >
          {label}
        </label>
      )}
    </div>
  );
};
