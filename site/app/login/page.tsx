"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { signIn } from "next-auth/react"
import { ThemeSwitch } from "@/components/theme-switch";
import LoginErrors from "@/server/messages/errors/login-errors";

export default function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const router = useRouter();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const result = await signIn("credentials", {
            redirect: false,
            username,
            password,
        });

        console.log(result);

        if (result?.error) {
            const { status } = JSON.parse(result.error) || {};
            const errorMessage = LoginErrors[status] || LoginErrors.Default;
            setError(errorMessage);
        } else {
            router.push("/");
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen dark:bg-gray-950 bg-gray-50">
            <div className="fixed top-4 right-4">
                <ThemeSwitch />
            </div>
            <div className="w-full max-w-md p-8 space-y-4 dark:bg-gray-900 bg-gray-100 rounded shadow">
                <h2 className="text-2xl font-bold text-center">
                    Авторизация
                </h2>
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label htmlFor="username" className="block text-sm font-medium">
                            Имя пользователя
                        </label>
                        <input
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            className="w-full px-3 py-2 border rounded focus:outline-none focus:ring"
                        />
                    </div>
                    <div>
                        <label htmlFor="password" className="block text-sm font-medium">
                            Пароль
                        </label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            className="w-full px-3 py-2 border rounded focus:outline-none focus:ring"
                        />
                    </div>
                    {error && <p className="text-red-500 text-sm">{error}</p>}
                    <button
                        type="submit"
                        className="w-full py-2 text-white bg-indigo-600 rounded hover:bg-indigo-700 focus:outline-none focus:ring"
                    >
                        Войти
                    </button>
                </form>
                <div className="text-center mt-4">
                    <p className="text-sm">
                        Нет аккаунта?{" "}
                        <button
                            onClick={() => router.push("/register")}
                            className="text-indigo-600 hover:underline"
                        >
                            Зарегистрироваться
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}
