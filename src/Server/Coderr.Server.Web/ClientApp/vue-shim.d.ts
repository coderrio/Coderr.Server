import Vue from 'vue'
import VueRouter from 'vue-router'
import {Route} from 'vue-router'

export interface IMyApplication {
    id: number;
    name: string;
    isAdmin: boolean;
}

export interface IUser {
    id: number;
    name: string;
    isSysAdmin: boolean;
    applications: IMyApplication[];
}

//declare module "ClientApp/components/*" {
//    import Vue from "vue"
//    export default Vue
//}

declare module 'vue/types/vue' {
    interface Vue {
        //$router: VueRouter;
        //$route: Route;
        user$: IUser;
    }
}