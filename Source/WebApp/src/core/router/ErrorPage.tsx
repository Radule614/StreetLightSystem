export const ErrorPage = () => {
  return (
    <div className="p-8 flex flex-col items-center [&>p]:max-w-[500px] text-center my-12">
      <p className="text-[32px] m-0 p-0 font-bold">
        <span className="text-blue-600">Oops!</span>&nbsp;Page Not Found
      </p>
      <p className="text-md mt-4">
        We're sorry, but the page you're looking for could not be found. It may
        have been moved, renamed, or deleted.
      </p>
    </div>
  );
};
