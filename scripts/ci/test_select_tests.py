import unittest
from unittest.mock import patch
import subprocess

from select_tests import plan, select


class SelectionTests(unittest.TestCase):
    def test_explanatory_root_docs_skip_dotnet(self):
        self.assertEqual(set(), select(['README.md', 'CONTRIBUTING.md']))

    def test_docs_keep_reference_checks(self):
        self.assertEqual(set(), select(['docs/security/README.md']))
        self.assertEqual({'analyzer'}, select(['docs/diagnostics/unsupported-syntax.md']))

    def test_executable_markdown_keeps_syntax_checks(self):
        self.assertEqual({'syntax', 'analyzer'}, select(['docs/csharp-syntax/csharp-14.md']))

    def test_analyzer_test_only(self):
        self.assertEqual({'analyzer'}, select(['tests/Neo.SmartContract.Analyzer.UnitTests/Test.cs']))

    def test_code_shared_configuration_and_unknown_paths_run_full(self):
        for path in ['src/Neo.Compiler.CSharp/Test.cs', 'src/Neo.SmartContract.Analyzer/Test.cs',
                     'src/Neo.SmartContract.Analyzer/AnalyzerReleases.Shipped.md',
                     'tests/Directory.Build.props', 'global.json', 'NuGet.Config',
                     '.github/workflows/main.yml', 'profiles/schema.json',
                     'tests/Neo.Compiler.CSharp.UnitTests/Test.cs', 'new-module/file']:
            with self.subTest(path=path):
                self.assertEqual({'full'}, select(['README.md', path]))

    def test_rename_out_of_code_keeps_old_path(self):
        self.assertEqual({'full'}, select(['src/Old.cs', 'docs/New.md']))

    def test_push_and_manual_runs_are_full(self):
        for event in ['push', 'workflow_dispatch', 'merge_group']:
            self.assertEqual({'full'}, plan(event, {}))

    def test_missing_pr_information_is_full(self):
        self.assertEqual({'full'}, plan('pull_request', {}))

    @patch('select_tests.subprocess.check_output', side_effect=subprocess.CalledProcessError(1, 'git'))
    def test_diff_failure_is_full(self, _):
        self.assertEqual({'full'}, plan('pull_request', self.event()))

    @patch('select_tests.subprocess.check_output', return_value=b'README.md\0docs/a b.md\0')
    def test_whole_pr_diff_and_nul_paths(self, run):
        self.assertEqual(set(), plan('pull_request', self.event()))
        self.assertEqual(['git', 'diff', '--name-only', '--no-renames', '-z', 'base...head', '--'], run.call_args.args[0])

    @staticmethod
    def event():
        return {'pull_request': {'base': {'sha': 'base'}, 'head': {'sha': 'head'}}}


if __name__ == '__main__':
    unittest.main()
