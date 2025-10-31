import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Eye, EyeOff } from 'lucide-react';
import { authApi } from './authApi';
import { useAuthStore } from '../../lib/authStore';

const registerSchema = z.object({
  fullName: z.string().min(2, 'Full name must be at least 2 characters'),
  email: z.string().email('Invalid email address'),
  password: z.string()
    .min(8, 'Password must be at least 8 characters long')
    .regex(/[A-Z]/, 'Password must contain at least one uppercase letter')
    .regex(/[a-z]/, 'Password must contain at least one lowercase letter')
    .regex(/\d/, 'Password must contain at least one number')
    .regex(/[\W_]/, 'Password must contain at least one special character (!@#$%^&*)'),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

type RegisterFormData = z.infer<typeof registerSchema>;

export function Register() {
  const navigate = useNavigate();
  const login = useAuthStore((state) => state.login);
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  });

  const onSubmit = async (data: RegisterFormData) => {
    try {
      setIsLoading(true);
      setError('');
      const { confirmPassword, ...registerData } = data;
      const response = await authApi.register(registerData);
      const user = {
        userId: response.userId,
        email: response.email,
        fullName: response.fullName,
      };
      login(user, response.accessToken, response.refreshToken);
      navigate('/dashboard');
    } catch (err: any) {
      console.error('Registration failed:', err);
      
      // Handle different error types
      if (err.response?.status === 404) {
        setError('Unable to connect to server. Please check the API URL configuration.');
      } else if (err.response?.data?.errors) {
        // Handle validation errors from backend
        const validationErrors = err.response.data.errors;
        const errorMessages = Object.values(validationErrors).flat().join('. ');
        setError(errorMessages);
      } else if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else if (err.response?.data?.title) {
        setError(err.response.data.title);
      } else if (err.message === 'Network Error') {
        setError('Network error. Please check your internet connection.');
      } else if (err.message) {
        setError(err.message);
      } else {
        setError('Registration failed. Please try again later.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary to-secondary/90 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo/Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center mb-4">
            {/* PathLock Logo */}
            <svg 
              width="64" 
              height="64" 
              viewBox="0 0 120 120" 
              fill="none"
              className="flex-shrink-0"
            >
              {/* LEFT VERTICAL STROKE */}
              <line
                x1="15"
                y1="60"
                x2="15"
                y2="95"
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
              />
              
              {/* OUTER hexagon ring - OPEN at bottom left - WIDER */}
              <path 
                d="
                  M 15 60
                  L 15 25
                  L 55 5
                  L 95 25
                  L 95 75
                  L 55 95
                  L 35 82
                "
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
                fill="none"
              />
              
              {/* Small vertical stroke at bottom-right */}
              <line
                x1="35"
                y1="82"
                x2="35"
                y2="95"
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
              />
              
              {/* INNER hexagon ring - OPEN at bottom left - WIDER */}
              <path 
                d="
                  M 28 58
                  L 28 35
                  L 55 20
                  L 82 35
                  L 82 65
                  L 55 80
                  L 42 73
                "
                stroke="#24B770"
                strokeWidth="8"
                strokeLinecap="round"
                fill="none"
              />
            </svg>
          </div>
          <h1 className="text-3xl font-bold text-white mb-2">Create Account</h1>
          <p className="text-gray-300">Join PathLock Project Manager</p>
        </div>

        {/* Register Form */}
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                {error}
              </div>
            )}

            {/* Full Name Field */}
            <div>
              <label htmlFor="fullName" className="block text-sm font-medium text-gray-700 mb-2">
                Full Name
              </label>
              <input
                {...register('fullName')}
                type="text"
                id="fullName"
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition"
                placeholder="John Doe"
              />
              {errors.fullName && (
                <p className="mt-1 text-sm text-red-600">{errors.fullName.message}</p>
              )}
            </div>

            {/* Email Field */}
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                Email Address
              </label>
              <input
                {...register('email')}
                type="email"
                id="email"
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition"
                placeholder="you@example.com"
              />
              {errors.email && (
                <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>
              )}
            </div>

            {/* Password Field */}
            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-2">
                Password
              </label>
              <div className="relative">
                <input
                  {...register('password')}
                  type={showPassword ? 'text' : 'password'}
                  id="password"
                  autoComplete="new-password"
                  className="w-full px-4 py-3 pr-12 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition [&::-ms-reveal]:hidden [&::-ms-clear]:hidden"
                  style={{ WebkitTextSecurity: showPassword ? 'none' : undefined } as any}
                  placeholder="••••••••"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition"
                  tabIndex={-1}
                >
                  {showPassword ? (
                    <EyeOff className="w-5 h-5" />
                  ) : (
                    <Eye className="w-5 h-5" />
                  )}
                </button>
              </div>
              {errors.password && (
                <p className="mt-1 text-sm text-red-600">{errors.password.message}</p>
              )}
              {!errors.password && (
                <p className="mt-1 text-xs text-gray-500">
                  Must be 8+ characters with uppercase, lowercase, number, and special character
                </p>
              )}
            </div>

            {/* Confirm Password Field */}
            <div>
              <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700 mb-2">
                Confirm Password
              </label>
              <div className="relative">
                <input
                  {...register('confirmPassword')}
                  type={showConfirmPassword ? 'text' : 'password'}
                  id="confirmPassword"
                  autoComplete="new-password"
                  className="w-full px-4 py-3 pr-12 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition [&::-ms-reveal]:hidden [&::-ms-clear]:hidden"
                  style={{ WebkitTextSecurity: showConfirmPassword ? 'none' : undefined } as any}
                  placeholder="••••••••"
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition"
                  tabIndex={-1}
                >
                  {showConfirmPassword ? (
                    <EyeOff className="w-5 h-5" />
                  ) : (
                    <Eye className="w-5 h-5" />
                  )}
                </button>
              </div>
              {errors.confirmPassword && (
                <p className="mt-1 text-sm text-red-600">{errors.confirmPassword.message}</p>
              )}
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              className="w-full bg-primary hover:bg-primary/90 text-white font-semibold py-3 px-4 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
              {isLoading ? (
                <>
                  <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  Creating account...
                </>
              ) : (
                'Create Account'
              )}
            </button>
          </form>

          {/* Login Link */}
          <div className="mt-6 text-center">
            <p className="text-gray-600">
              Already have an account?{' '}
              <Link to="/login" className="text-primary hover:text-primary/80 font-semibold">
                Sign in
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
