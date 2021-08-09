const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssoWebpackPlugin = require('csso-webpack-plugin').default;

// Require fontawesome-subset
const fontawesomeSubset = require('fontawesome-subset');

// Create or append a task to be ran with your configuration
fontawesomeSubset([
  'tachometer-alt',
  'folder',
  'copyright',
  'edit',
  'list',
  'plus',
  'th',
  'users',
  'gamepad',
  'comments',
  'file',
  'globe',
  'tags',
  'database',
  'trash',
  'copy',
  'upload',
  'folder-plus',
  'sync'], 'src/webfonts');

module.exports = {
  entry: {
    index: path.resolve(__dirname, 'src', 'index.js'),
  },
  output: {
    filename: '[name].js',
    path: path.resolve(__dirname, '..', 'wwwroot', 'dist'),
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: "[name].css"
    }),
    new CssoWebpackPlugin()
  ],
  module: {
    rules: [
      {
        test: /\.(sa|sc)ss$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
            options: {
              publicPath: '/dist/'
            }
          },
          {
            loader: 'css-loader', // translates CSS into CommonJS modules
          }, {
            loader: 'postcss-loader', // Run post css actions
            options: {
              postcssOptions: {
                plugins: function () { // post css plugins, can be exported to postcss.config.js
                  return [
                    require('precss'),
                    require('autoprefixer')
                  ];
                }
              }
            }
          }, {
            loader: 'sass-loader' // compiles Sass to CSS
          }]
      },
      {
        test: /\.(svg|eot|woff|woff2|ttf)$/,
        type: 'asset/resource'
      }
    ]
  }
};
