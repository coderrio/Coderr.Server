const webpackBaseConfig = require('./webpack.config.js');
const merge = require('webpack-merge');
const webpack = require("webpack");
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer')
    .BundleAnalyzerPlugin;

const configProd = {
    mode: 'production',
    devtool: 'source-map',
    output: {
        //chunkFilename: '[name].min.js',
        //filename: '[name].min.js'
    },
    optimization: {
        usedExports: true,
        minimizer: [
            new TerserPlugin(),
            new OptimizeCssAssetsPlugin()
        ],
        splitChunks: {
            cacheGroups: {
                vendor: {
                    test: /[\\/]node_modules[\\/]/,
                    name: "vendor",
                    enforce: true,

                    /*
                        Entry chunks - Entry chunks contain webpack runtime and modules it then loads.
                        Normal chunks - Normal chunks don't contain webpack runtime. Instead, these can be loaded dynamically while the application is running. A suitable wrapper (JSONP for example) is generated for these. You generate a normal chunk in the next chapter as you set up code splitting.
                        Initial chunks - Initial chunks are normal chunks that count towards initial loading time of the application. As a user, you don't have to care about these. It's the split between entry chunks and normal chunks that is important.
                    */
                    chunks: "initial"
                }
            }
        }
    },
    plugins: [
        //new BundleAnalyzerPlugin(),
        new webpack.LoaderOptionsPlugin({
            minimize: true
        }),
        new OptimizeCssAssetsPlugin({
            assetNameRegExp: /\.optimize\.css$/g,
            cssProcessor: require('cssnano'),
            cssProcessorPluginOptions: {
                preset: ['default', { discardComments: { removeAll: true } }]
            },
            canPrint: true
        })
    ]
};

module.exports = merge(webpackBaseConfig, configProd);
