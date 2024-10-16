import { signIn, providerMap } from "@/auth"

export default async function SignInPage(props: {
  searchParams: { callbackUrl: string | undefined }
}) {
  return (
    <div className="flex flex-col gap-2">
      <form
        action={async (formData) => {
          "use server"
          await signIn("credentials", formData)
        }}
      >
        <label htmlFor="email">
          Почта
          <input name="email" id="email" />
        </label>
        <label htmlFor="password">
          Пароль
          <input name="password" id="password" />
        </label>
        <input type="submit" value="Войти" />
      </form>
      {Object.values(providerMap).map((provider) => (
        <form
          action={async () => {
            "use server"
            await signIn(provider.id, {
              redirectTo: props.searchParams?.callbackUrl ?? "",
            })
          }}
        >
          <button type="submit">
            <span>Войти с помощью {provider.name}</span>
          </button>
        </form>
      ))}
    </div>
  )
}