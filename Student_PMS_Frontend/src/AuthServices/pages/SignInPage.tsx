import PageMeta from "@/components/common/PageMeta";
import AuthLayout from "@/pages/AuthPages/AuthPageLayout";
import SignInForm from "@/AuthServices/components/SignInForm";

const SignInPage = () => (
  <>
    <PageMeta
      title="Sign In | Student PMS Frontend"
      description="Sign in to access the Student Project Management System frontend."
    />
    <AuthLayout>
      <SignInForm />
    </AuthLayout>
  </>
);

export default SignInPage;
