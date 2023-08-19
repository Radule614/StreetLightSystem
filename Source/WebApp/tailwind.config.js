/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "./node_modules/flowbite/**/*.js",
    "node_modules/flowbite-react/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    colors: {
      "dark-1": "#2b323f",
      "dark-2": "#3d4656",
      "dark-3": "#9ca3af",
      "light-1": "#ffffff",
      "light-2": "#f9fafb",
      "light-3": "#ececed",
    },
  },
  plugins: [require("flowbite/plugin")],
};
