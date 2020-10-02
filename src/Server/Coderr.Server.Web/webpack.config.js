const path = require("path");
var webpack = require("webpack");
const isDevBuild = process.env.NODE_ENV === "development" || process.argv.indexOf("--mode=development") > -1;
var TsconfigPathsPlugin = require("tsconfig-paths-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const { VueLoaderPlugin } = require("vue-loader");

var bundleOutputDir = "./wwwroot/dist";

function resolve(dir) {
    return path.join(__dirname, dir);
}
//TODO: include jquery
const baseConfig = {
    entry: {
        'main': "./ClientApp/boot.ts",
        "styles": [
            "./wwwroot/scss/bootstrap-coderr.scss",
            "./wwwroot/scss/checkbox.scss",
            "./wwwroot/scss/radio.scss",
            "./wwwroot/scss/common.scss",
            "./wwwroot/scss/site.scss"
        ]
    },
    stats: 'normal',
    output: {
        path: resolve(bundleOutputDir),
        publicPath: "dist/"
    },
    resolve: {
        extensions: [".ts", ".js", ".vue.html"],
        alias: {
            vue$: isDevBuild ? "vue/dist/vue.esm.js" : "vue/dist/vue.runtime.esm.js",
            '@': resolve("/ClientApp/")
        },
        plugins: [new TsconfigPathsPlugin({})]
    },
    module: {
        rules: [
            {
                test: /\.vue.html$/,
                use: {
                    loader: "vue-loader"
                }
            },
            {
                test: /\.ts$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: "ts-loader",
                        options: {
                            appendTsSuffixTo: [/\.vue.html$/]
                        }
                    }
                ]
            },
            {
                test: /\.css$/,
                use: isDevBuild
                    ? ['style-loader', 'css-loader']
                    : [MiniCssExtractPlugin.loader, 'css-loader']
            },
            {
                test: /\.scss$/,
                exclude: /coderr-variables.scss/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    'sass-loader'
                ]
            },
            {
                test: /\.(eot|svg|ttf|woff|woff2)$/,
                loader: "url-loader"
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg)$/,
                use: "url-loader?limit=25000"
            }
        ]
    },
    plugins: [
        new CleanWebpackPlugin(),
        new VueLoaderPlugin(),
        new MiniCssExtractPlugin(),
        new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: isDevBuild ? "'development'" : "'production'"
            }
        }),
        {
            apply(compiler) {
                compiler.hooks.shouldEmit.tap('Remove styles.js from output',
                    (compilation) => {
                        delete compilation.assets['styles.js', 'styles.js.map'];
                        return true;
                    });
            }
        }
    ]
};

module.exports = baseConfig;
