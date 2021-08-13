module.exports = {
  verifyConditions: ['@semantic-release/github'],
  plugins: [
    '@semantic-release/commit-analyzer',
    '@semantic-release/release-notes-generator',
        [
      "@semantic-release/changelog",
      {
        "changelogFile": "CHANGELOG.md"
      }
    ],
    [
      "@semantic-release/git",
      {
        "assets": ["CHANGELOG.md"]
      }
    ],
    '@semantic-release/github'
  ],
  branches: [
    '+([0-9])?(.{+([0-9]),x}).x',
    'main',
    { name: 'beta', prerelease: true },
  ]
};
