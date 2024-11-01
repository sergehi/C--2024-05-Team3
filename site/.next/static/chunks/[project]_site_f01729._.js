(globalThis.TURBOPACK = globalThis.TURBOPACK || []).push(["static/chunks/[project]_site_f01729._.js", {

"[project]/site/auth.ts [app-client] (ecmascript)": (({ r: __turbopack_require__, f: __turbopack_module_context__, i: __turbopack_import__, s: __turbopack_esm__, v: __turbopack_export_value__, n: __turbopack_export_namespace__, c: __turbopack_cache__, M: __turbopack_modules__, l: __turbopack_load__, j: __turbopack_dynamic__, P: __turbopack_resolve_absolute_path__, U: __turbopack_relative_url__, R: __turbopack_resolve_module_id_path__, g: global, __dirname, k: __turbopack_refresh__ }) => (() => {
"use strict";

__turbopack_esm__({
    "auth": ()=>auth,
    "handlers": ()=>handlers,
    "providerMap": ()=>providerMap,
    "signIn": ()=>signIn,
    "signOut": ()=>signOut
});
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2d$auth$2f$index$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__$3c$module__evaluation$3e$__ = __turbopack_import__("[project]/site/node_modules/next-auth/index.js [app-client] (ecmascript) <module evaluation>");
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2d$auth$2f$index$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__$3c$locals$3e$__ = __turbopack_import__("[project]/site/node_modules/next-auth/index.js [app-client] (ecmascript) <locals>");
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2d$auth$2f$providers$2f$google$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__$3c$module__evaluation$3e$__ = __turbopack_import__("[project]/site/node_modules/next-auth/providers/google.js [app-client] (ecmascript) <module evaluation>");
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f40$auth$2f$core$2f$providers$2f$google$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__ = __turbopack_import__("[project]/site/node_modules/@auth/core/providers/google.js [app-client] (ecmascript)");
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2d$auth$2f$providers$2f$credentials$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__$3c$module__evaluation$3e$__ = __turbopack_import__("[project]/site/node_modules/next-auth/providers/credentials.js [app-client] (ecmascript) <module evaluation>");
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f40$auth$2f$core$2f$providers$2f$credentials$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__ = __turbopack_import__("[project]/site/node_modules/@auth/core/providers/credentials.js [app-client] (ecmascript)");
"__TURBOPACK__ecmascript__hoisting__location__";
;
;
;
const providers = [
    (0, __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f40$auth$2f$core$2f$providers$2f$credentials$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__["default"])({
        credentials: {
            password: {
                label: "Пароль",
                type: "password"
            }
        },
        authorize (c) {
            if (c.password !== "password") return null;
            return {
                id: "test",
                name: "Test User",
                email: "test@example.com"
            };
        }
    }),
    __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f40$auth$2f$core$2f$providers$2f$google$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__["default"]
];
const providerMap = providers.map((provider)=>{
    if (typeof provider === "function") {
        const providerData = provider();
        return {
            id: providerData.id,
            name: providerData.name
        };
    } else {
        return {
            id: provider.id,
            name: provider.name
        };
    }
}).filter((provider)=>provider.id !== "credentials");
const { handlers, signIn, signOut, auth } = (0, __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2d$auth$2f$index$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__$3c$locals$3e$__["default"])({
    providers,
    callbacks: {
        async redirect ({ url, baseUrl }) {
            if (url === '/api/auth/signout') {
                return '/login'; // перенаправляем на страницу логина после выхода
            }
            return baseUrl;
        }
    },
    pages: {
        signIn: '/login'
    }
});

})()),
"[project]/site/components/sign-out.tsx [app-client] (ecmascript)": (({ r: __turbopack_require__, f: __turbopack_module_context__, i: __turbopack_import__, s: __turbopack_esm__, v: __turbopack_export_value__, n: __turbopack_export_namespace__, c: __turbopack_cache__, M: __turbopack_modules__, l: __turbopack_load__, j: __turbopack_dynamic__, P: __turbopack_resolve_absolute_path__, U: __turbopack_relative_url__, R: __turbopack_resolve_module_id_path__, g: global, __dirname, k: __turbopack_refresh__ }) => (() => {
"use strict";

/* __next_internal_action_entry_do_not_use__ {"298dff26fe415fa0306c8387f5d67fb06175ff31":"$$ACTION_0"} */ __turbopack_esm__({
    "$$ACTION_0": ()=>$$ACTION_0,
    "default": ()=>SignOut
});
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2f$dist$2f$compiled$2f$react$2f$jsx$2d$dev$2d$runtime$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__ = __turbopack_import__("[project]/site/node_modules/next/dist/compiled/react/jsx-dev-runtime.js [app-client] (ecmascript)");
var __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$auth$2e$ts__$5b$app$2d$client$5d$__$28$ecmascript$29$__ = __turbopack_import__("[project]/site/auth.ts [app-client] (ecmascript)");
"__TURBOPACK__ecmascript__hoisting__location__";
__turbopack_refresh__.register(_c, "SignOut");
;
;
function SignOut() {
    return /*#__PURE__*/ (0, __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2f$dist$2f$compiled$2f$react$2f$jsx$2d$dev$2d$runtime$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__["jsxDEV"])("form", {
        action: registerServerReference("298dff26fe415fa0306c8387f5d67fb06175ff31", $$ACTION_0),
        children: /*#__PURE__*/ (0, __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$node_modules$2f$next$2f$dist$2f$compiled$2f$react$2f$jsx$2d$dev$2d$runtime$2e$js__$5b$app$2d$client$5d$__$28$ecmascript$29$__["jsxDEV"])("button", {
            type: "submit",
            children: "Выйти"
        }, void 0, false, {
            fileName: "[project]/site/components/sign-out.tsx",
            lineNumber: 11,
            columnNumber: 13
        }, this)
    }, void 0, false, {
        fileName: "[project]/site/components/sign-out.tsx",
        lineNumber: 5,
        columnNumber: 9
    }, this);
}
async function $$ACTION_0(formData) {
    await (0, __TURBOPACK__imported__module__$5b$project$5d2f$site$2f$auth$2e$ts__$5b$app$2d$client$5d$__$28$ecmascript$29$__["signOut"])();
}
_c = SignOut;
var _c;

})()),
"[project]/site/app/page.tsx [app-rsc] (ecmascript, Next.js server component, client modules)": (({ r: __turbopack_require__, f: __turbopack_module_context__, i: __turbopack_import__, s: __turbopack_esm__, v: __turbopack_export_value__, n: __turbopack_export_namespace__, c: __turbopack_cache__, M: __turbopack_modules__, l: __turbopack_load__, j: __turbopack_dynamic__, P: __turbopack_resolve_absolute_path__, U: __turbopack_relative_url__, R: __turbopack_resolve_module_id_path__, g: global, __dirname }) => (() => {


})()),
}]);

//# sourceMappingURL=%5Bproject%5D_site_f01729._.js.map