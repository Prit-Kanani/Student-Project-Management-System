import PageMeta from "@/components/common/PageMeta";
import AuthLayout from "@/pages/AuthPages/AuthPageLayout";
import SignUpForm from "@/AuthServices/components/SignUpForm";

const SignUpPage = () => (
  <>
    <PageMeta
      title="Register | Student PMS Frontend"
      description="Create an account for the Student Project Management System frontend."
    />
    <AuthLayout>
      <SignUpForm />
    </AuthLayout>
  </>
);

export default SignUpPage;
