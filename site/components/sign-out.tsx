import { signOut } from "@/auth"

export default function SignOut() {
    return (
        <form
            action={async (formData) => {
                "use server"
                await signOut({ redirectTo: "/login", redirect: true })
            }}
        >
            <button type="submit">Выйти</button>
        </form>
    )
} 