"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ThemeSwitch } from "@/components/theme-switch";
import RegisterErrors from "@/server/messages/errors/register-errors";
import ApiUrls from "@/config/api-urls";

export default function RegisterPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const router = useRouter();

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        if (password !== confirmPassword) {
            setError("Пароли не совпадают");
            return;
        }

        try {
            const response = await fetch("/api/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password }),
            });

            const data = await response.json();

            console.log(data);

            if (response.status === 200) {
                router.push("/login");
            } else {
                const errorMessage = RegisterErrors[response.status] || RegisterErrors.Default;
                setError(errorMessage);
            }
        } catch (error) {
            setError(RegisterErrors.Default);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen dark:bg-gray-950 bg-gray-50">
            <div className="fixed top-4 right-4">
                <ThemeSwitch />
            </div>
            <div className="w-full max-w-md p-8 space-y-4 dark:bg-gray-900 bg-gray-100 rounded shadow">
                <h2 className="text-2xl font-bold text-center">
                    Регистрация
                </h2>
                <form onSubmit={handleRegister} className="space-y-4">
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
                    <div>
                        <label htmlFor="confirmPassword" className="block text-sm font-medium">
                            Подтвердите пароль
                        </label>
                        <input
                            id="confirmPassword"
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            required
                            className="w-full px-3 py-2 border rounded focus:outline-none focus:ring"
                        />
                    </div>
                    {error && <p className="text-red-500 text-sm">{error}</p>}
                    <button
                        type="submit"
                        className="w-full py-2 text-white bg-indigo-600 rounded hover:bg-indigo-700 focus:outline-none focus:ring"
                    >
                        Зарегистрироваться
                    </button>
                </form>
                <div className="text-center mt-4">
                    <p className="text-sm">
                        Уже есть аккаунт?{" "}
                        <button
                            onClick={() => router.push("/login")}
                            className="text-indigo-600 hover:underline"
                        >
                            Войти
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}