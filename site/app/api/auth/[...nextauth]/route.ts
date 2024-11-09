import NextAuth, { NextAuthOptions, User } from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";
import ApiUrls from "@/config/api-urls";

interface ExtendedSession {
    accessToken?: unknown;
    refreshToken?: unknown;
}

interface ExtendedUser extends User {
    accessToken?: string;
    refreshToken?: string;
}

const authOptions: NextAuthOptions = {
    providers: [
        CredentialsProvider({
            name: 'Credentials',
            credentials: {
                username: { label: 'Username', type: 'text' },
                password: { label: 'Password', type: 'password' },
            },
            authorize: async (credentials) => {
                try {
                    const response = await fetch(ApiUrls.authorizationService.login, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            username: credentials?.username,
                            password: credentials?.password,
                        }),
                    });

                    const data = await response.json();

                    console.log(data);

                    if (response.ok) {
                        return data;
                    }

                    throw new Error(JSON.stringify({ status: response.status, message: data.message }));
                } catch (error) {
                    return Promise.reject(error);
                }
            },
        }),
    ],
    session: {
        strategy: "jwt",
    },
    callbacks: {
        async jwt({ token, user }) {
            if (user) {
                const extendedUser = user as ExtendedUser;
                token.accessToken = extendedUser.accessToken;
                token.refreshToken = extendedUser.refreshToken;
            }
            return token;
        },
        async session({ session, token }) {
            const extendedSession = session as ExtendedSession;
            extendedSession.accessToken = token.accessToken;
            extendedSession.refreshToken = token.refreshToken;
            return session;
        },
    },
    pages: {
        signIn: '/login',
    },
};

export const GET = NextAuth(authOptions);
export const POST = NextAuth(authOptions);