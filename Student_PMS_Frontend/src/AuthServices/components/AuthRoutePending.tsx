const AuthRoutePending = ({ message }: { message: string }) => (
  <div className="flex min-h-screen items-center justify-center bg-gray-50 px-6 dark:bg-gray-950">
    <div className="rounded-3xl border border-gray-200 bg-white px-8 py-6 text-center shadow-theme-lg dark:border-gray-800 dark:bg-gray-900">
      <div className="mx-auto size-10 animate-spin rounded-full border-4 border-brand-100 border-t-brand-500" />
      <p className="mt-4 text-sm font-medium text-gray-700 dark:text-gray-300">
        {message}
      </p>
    </div>
  </div>
);

export default AuthRoutePending;
